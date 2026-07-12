using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;

namespace Marketplace.Tests.E2E;

public sealed class MarketplaceE2eFixture : IAsyncLifetime
{
    private readonly string _databaseName = $"DotNetMarketplace_E2E_{Guid.NewGuid():N}";
    private ManagedHostProcess? _apiProcess;
    private ManagedHostProcess? _webProcess;
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public string SolutionRoot { get; } = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
    public int ApiPort { get; } = GetFreeTcpPort();
    public int WebPort { get; } = GetFreeTcpPort();
    public string ApiBaseUrl => $"http://127.0.0.1:{ApiPort}/";
    public string WebBaseUrl => $"http://127.0.0.1:{WebPort}/";
    public string AdminUserName => "michael";
    public string AdminPassword => "ChangeMe123!";

    private string ConnectionString =>
        $"Server=(localdb)\\SGPLocalDB;Database={_databaseName};Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

    public async Task InitializeAsync()
    {
        _apiProcess = ManagedHostProcess.Start(
            workingDirectory: SolutionRoot,
            projectPath: "src/Marketplace.Api/Marketplace.Api.csproj",
            baseUrl: ApiBaseUrl,
            new Dictionary<string, string?>
            {
                ["ASPNETCORE_ENVIRONMENT"] = "Development",
                ["DOTNET_ENVIRONMENT"] = "Development",
                ["ConnectionStrings__Marketplace"] = ConnectionString,
                ["SeedDatabase"] = "true"
            });

        await WaitForHttpSuccessAsync(new Uri(new Uri(ApiBaseUrl), "api/health"), _apiProcess, TimeSpan.FromSeconds(90));

        _webProcess = ManagedHostProcess.Start(
            workingDirectory: SolutionRoot,
            projectPath: "src/Marketplace.Web/Marketplace.Web.csproj",
            baseUrl: WebBaseUrl,
            new Dictionary<string, string?>
            {
                ["ASPNETCORE_ENVIRONMENT"] = "Development",
                ["DOTNET_ENVIRONMENT"] = "Development",
                ["ApiBaseUrl"] = ApiBaseUrl
            });

        await WaitForHttpSuccessAsync(new Uri(new Uri(WebBaseUrl), "login"), _webProcess, TimeSpan.FromSeconds(60));

        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Channel = "msedge",
            Headless = true
        });
    }

    public async Task<BrowserSession> CreateSessionAsync()
    {
        ArgumentNullException.ThrowIfNull(_browser);

        var context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = WebBaseUrl
        });

        var page = await context.NewPageAsync();
        page.SetDefaultTimeout(15000);
        page.SetDefaultNavigationTimeout(30000);

        return new BrowserSession(context, page);
    }

    public async Task<int> WaitForProductIdBySkuAsync(string sku, TimeSpan? timeout = null)
    {
        var startedAt = DateTimeOffset.UtcNow;
        var limit = timeout ?? TimeSpan.FromSeconds(15);

        while (DateTimeOffset.UtcNow - startedAt < limit)
        {
            await using var db = CreateDbContext();
            var productId = await db.Products
                .Where(item => item.Sku == sku)
                .Select(item => (int?)item.Id)
                .FirstOrDefaultAsync();

            if (productId is not null)
            {
                return productId.Value;
            }

            await Task.Delay(500);
        }

        throw new TimeoutException($"Produto com SKU {sku} nao apareceu no banco de teste.");
    }

    public async Task DisposeAsync()
    {
        if (_browser is not null)
        {
            await _browser.CloseAsync();
        }

        _playwright?.Dispose();

        if (_webProcess is not null)
        {
            await _webProcess.DisposeAsync();
        }

        if (_apiProcess is not null)
        {
            await _apiProcess.DisposeAsync();
        }

        await using var db = CreateDbContext();
        await db.Database.EnsureDeletedAsync();
    }

    private MarketplaceDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MarketplaceDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        return new MarketplaceDbContext(options);
    }

    private static async Task WaitForHttpSuccessAsync(Uri uri, ManagedHostProcess process, TimeSpan timeout)
    {
        using var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(5)
        };

        var startedAt = DateTimeOffset.UtcNow;
        Exception? lastError = null;

        while (DateTimeOffset.UtcNow - startedAt < timeout)
        {
            if (process.HasExited)
            {
                throw new InvalidOperationException(
                    $"Processo finalizou antes do endpoint responder em {uri}.{Environment.NewLine}{process.GetDiagnostics()}");
            }

            try
            {
                using var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    return;
                }

                lastError = new HttpRequestException($"Status {(int)response.StatusCode} ao acessar {uri}.");
            }
            catch (Exception ex)
            {
                lastError = ex;
            }

            await Task.Delay(1000);
        }

        throw new TimeoutException(
            $"Tempo esgotado aguardando {uri}.{Environment.NewLine}{process.GetDiagnostics()}{Environment.NewLine}Ultimo erro: {lastError}");
    }

    private static int GetFreeTcpPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();

        try
        {
            return ((IPEndPoint)listener.LocalEndpoint).Port;
        }
        finally
        {
            listener.Stop();
        }
    }

    private sealed class ManagedHostProcess : IAsyncDisposable
    {
        private readonly Process _process;
        private readonly StringBuilder _output = new();
        private readonly object _sync = new();

        private ManagedHostProcess(Process process)
        {
            _process = process;
        }

        public bool HasExited => _process.HasExited;

        public static ManagedHostProcess Start(string workingDirectory, string projectPath, string baseUrl, IReadOnlyDictionary<string, string?> environmentVariables)
        {
            var startInfo = new ProcessStartInfo("dotnet", $"run --project {projectPath} --no-build --urls {baseUrl}")
            {
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            foreach (var (key, value) in environmentVariables)
            {
                startInfo.Environment[key] = value;
            }

            var process = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            var managed = new ManagedHostProcess(process);
            process.OutputDataReceived += (_, args) => managed.Append(args.Data);
            process.ErrorDataReceived += (_, args) => managed.Append(args.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return managed;
        }

        public string GetDiagnostics()
        {
            lock (_sync)
            {
                return _output.ToString();
            }
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                if (!_process.HasExited)
                {
                    _process.Kill(entireProcessTree: true);
                    await _process.WaitForExitAsync();
                }
            }
            catch (InvalidOperationException)
            {
                // Processo ja finalizado.
            }
            finally
            {
                _process.Dispose();
            }
        }

        private void Append(string? line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return;
            }

            lock (_sync)
            {
                _output.AppendLine(line);
            }
        }
    }
}
