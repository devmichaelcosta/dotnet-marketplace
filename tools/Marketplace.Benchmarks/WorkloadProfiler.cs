using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Claims;
using Marketplace.Api.Domain;
using Marketplace.Api.Infrastructure.Persistence;
using Marketplace.Api.Modules.Catalog.Website.GetById;
using Marketplace.Api.Modules.Catalog.Website.SearchProducts;
using Marketplace.Api.Modules.Cart.Website.Checkout;
using Marketplace.Api.Modules.Orders.Admin.Search;
using Marketplace.Api.Modules.ProductImports.Admin.Shared;
using Marketplace.Api.Modules.Produto.Admin.Search;
using Marketplace.Api.Modules.Produto.Admin.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NPOI.XSSF.UserModel;

public sealed class WorkloadProfiler(DbContextOptions<MarketplaceDbContext> options, BenchmarkDataset dataset)
{
    private const int Repetitions = 3;
    private static readonly string[] ImportHeaders = ["Titulo", "LoginVendedor", "PrecoAVista", "Sku", "Estoque", "EhOferta", "Categoria", "Subcategoria", "Descritivo", "Imagens"];

    public async Task<object> RunAsync(string sqlServerVersion, CancellationToken cancellationToken)
    {
        var userIds = dataset.UserIds;
        var publicSearch = await MeasureAsync("Busca publica do catalogo", async () =>
        {
            await using var db = CreateDbContext();
            var handler = new Marketplace.Api.Modules.Catalog.Website.SearchProducts.SearchProductsHandler(db);
            await handler.HandleAsync(new Marketplace.Api.Modules.Catalog.Website.SearchProducts.SearchProductsQuery { Search = "Benchmark Product", Page = 1 }, cancellationToken);
        });
        var details = await MeasureAsync("Detalhe publico do produto", async () =>
        {
            await using var db = CreateDbContext();
            await new GetCatalogProductByIdHandler(db).HandleAsync(dataset.ProductIds[250], cancellationToken);
        });
        var adminProducts = await MeasureAsync("Listagem administrativa de produtos", async () =>
        {
            await using var db = CreateDbContext();
            var context = CreateHttpContext(dataset.AdminId, "Admin");
            var handler = new Marketplace.Api.Modules.Produto.Admin.Search.SearchProductsHandler(db, new ProductAdminAccessPolicy());
            await handler.HandleAsync(new Marketplace.Api.Modules.Produto.Admin.Search.SearchProductsQuery { Search = "Benchmark Product", Page = 1, PageSize = 50, Sort = "title", Direction = "asc" }, context, cancellationToken);
        });
        var adminOrders = await MeasureAsync("Listagem administrativa de pedidos", async () =>
        {
            await using var db = CreateDbContext();
            var handler = new SearchAdminOrdersHandler(db);
            await handler.HandleAsync(new SearchAdminOrdersQuery { Search = "Synthetic Customer", Sort = "created", Direction = "desc" }, cancellationToken);
        });
        var import = await MeasureAsync("Importacao de 500 linhas com imagem local simulada", () => ImportFiveHundredRowsAsync(cancellationToken));
        var checkout = new List<CheckoutConsistencyResult>();
        for (var repetition = 0; repetition < Repetitions; repetition++)
        {
            checkout.Add(await RunCheckoutConcurrencyAsync(repetition, cancellationToken));
        }

        await using var countDb = CreateDbContext();
        var datasetCounts = new
        {
            Products = await countDb.Products.CountAsync(cancellationToken),
            Users = await countDb.Users.CountAsync(cancellationToken),
            Orders = await countDb.Orders.CountAsync(cancellationToken),
            OrderItems = await countDb.OrderItems.CountAsync(cancellationToken)
        };
        var memory = GC.GetGCMemoryInfo();
        return new
        {
            GeneratedAtUtc = DateTimeOffset.UtcNow,
            Environment = new
            {
                OperatingSystem = RuntimeInformation.OSDescription,
                Runtime = RuntimeInformation.FrameworkDescription,
                ProcessorCount = Environment.ProcessorCount,
                AvailableMemoryBytes = memory.TotalAvailableMemoryBytes,
                SqlServerVersion = sqlServerVersion.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()
            },
            Dataset = datasetCounts,
            Repetitions,
            Measurements = new[] { publicSearch, details, adminProducts, adminOrders, import },
            CheckoutConcurrency = checkout,
            Limitations = new[] { "Medições executam os handlers no processo do benchmark: incluem acesso ao EF/SQL e regra do caso de uso, mas excluem rede HTTP, TLS, proxy, renderização Blazor e variabilidade de uma infraestrutura de produção.", "CPU e memória são os recursos disponíveis do host no momento da execução; o relatório não atribui consumo exclusivo por consulta." }
        };
    }

    private MarketplaceDbContext CreateDbContext() => new(options);

    private static DefaultHttpContext CreateHttpContext(Guid userId, string role)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, "benchmark-admin"),
            new Claim(ClaimTypes.Role, role)
        ], "benchmark");
        return new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
    }

    private static async Task<WorkloadMeasurement> MeasureAsync(string name, Func<Task> operation)
    {
        var samples = new double[Repetitions];
        for (var index = 0; index < Repetitions; index++)
        {
            var stopwatch = Stopwatch.StartNew();
            await operation();
            stopwatch.Stop();
            samples[index] = stopwatch.Elapsed.TotalMilliseconds;
        }

        var ordered = samples.Order().ToArray();
        var seconds = samples.Sum() / 1000d;
        return new WorkloadMeasurement(
            name,
            Percentile(ordered, 0.50),
            Percentile(ordered, 0.95),
            Percentile(ordered, 0.99),
            samples.Length / seconds,
            ErrorCount: 0,
            TimeoutCount: 0,
            samples);
    }

    private static double Percentile(double[] values, double percentile) =>
        values[Math.Clamp((int)Math.Ceiling(values.Length * percentile) - 1, 0, values.Length - 1)];

    private async Task<double> ImportFiveHundredRowsAsync(CancellationToken cancellationToken)
    {
        var repetition = Interlocked.Increment(ref _importSequence);
        var webRoot = Path.GetFullPath(Path.Combine(".tmp", "performance-baseline", "wwwroot"));
        var contentRoot = Path.GetFullPath(Path.Combine(".tmp", "performance-baseline"));
        var environment = new BenchmarkWebHostEnvironment(contentRoot, webRoot);
        var bytes = CreateImportWorkbook(repetition);
        var relativePath = Path.Combine("uploads", "product-imports", "benchmark-" + repetition, "original.xlsx");
        var absolutePath = Path.Combine(webRoot, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);
        await File.WriteAllBytesAsync(absolutePath, bytes, cancellationToken);

        int jobId;
        await using (var db = CreateDbContext())
        {
            var job = new ProductImportJob
            {
                OriginalFileName = "benchmark-500.xlsx",
                StoredFileName = "original.xlsx",
                StoredFilePath = "/" + relativePath.Replace('\\', '/'),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileSizeBytes = bytes.Length,
                ImportedByUserId = dataset.AdminId,
                ImportedByName = "benchmark-admin",
                Status = ProductImportJobStatus.Pending,
                SummaryMessage = "Synthetic benchmark import."
            };
            db.ProductImportJobs.Add(job);
            await db.SaveChangesAsync(cancellationToken);
            jobId = job.Id;
        }

        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "src", "Marketplace.Api", "wwwroot", "uploads", "products", "Ibanez-RG-7420Z.jpg");
        var imageBytes = await File.ReadAllBytesAsync(imagePath, cancellationToken);
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Error);
            builder.AddConsole();
        });
        await using var importDb = CreateDbContext();
        var downloader = new ProductImportImageDownloader(new BenchmarkHttpClientFactory(imageBytes), environment);
        var processor = new ProductImportProcessor(importDb, environment, downloader, loggerFactory.CreateLogger<ProductImportProcessor>());
        var stopwatch = Stopwatch.StartNew();
        await processor.ProcessAsync(jobId, cancellationToken);
        stopwatch.Stop();

        var completed = await importDb.ProductImportJobs.AsNoTracking().FirstAsync(item => item.Id == jobId, cancellationToken);
        if (completed.Status != ProductImportJobStatus.Success || completed.SkuCount != 500 || completed.ErrorCount != 0)
        {
            throw new InvalidOperationException("Import benchmark esperava 500 linhas validas; status=" + completed.Status + ", sku=" + completed.SkuCount + ", erros=" + completed.ErrorCount);
        }

        return stopwatch.Elapsed.TotalMilliseconds;
    }

    private byte[] CreateImportWorkbook(int repetition)
    {
        using var workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet("Produtos");
        var header = sheet.CreateRow(0);
        for (var index = 0; index < ImportHeaders.Length; index++)
        {
            header.CreateCell(index).SetCellValue(ImportHeaders[index]);
        }

        for (var index = 0; index < 500; index++)
        {
            var row = sheet.CreateRow(index + 1);
            var values = new[]
            {
                "Benchmark Import " + repetition + " " + index,
                "benchmark-user-0000",
                "19,90",
                "BENCH-IMPORT-" + repetition.ToString("D2") + "-" + index.ToString("D4"),
                "25",
                index % 2 == 0 ? "sim" : "nao",
                "Benchmark Catalog",
                "Benchmark Items",
                "Synthetic product import row used by the isolated benchmark dataset.",
                "https://benchmark.invalid/image.jpg"
            };
            for (var cell = 0; cell < values.Length; cell++)
            {
                row.CreateCell(cell).SetCellValue(values[cell]);
            }
        }

        using var stream = new MemoryStream();
        workbook.Write(stream, true);
        return stream.ToArray();
    }

    private int _importSequence;

    private async Task<CheckoutConsistencyResult> RunCheckoutConcurrencyAsync(int repetition, CancellationToken cancellationToken)
    {
        const int stock = 10;
        const int attempts = 30;
        var productId = dataset.ProductIds[0];
        var buyers = dataset.UserIds.Skip(1000 + repetition * attempts).Take(attempts).ToArray();
        var price = 20m;
        await using (var setup = CreateDbContext())
        {
            var product = await setup.Products.SingleAsync(item => item.Id == productId, cancellationToken);
            product.Stock = stock;
            setup.Carts.AddRange(buyers.Select(userId => new Marketplace.Api.Domain.Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                AnonymousKey = Guid.NewGuid().ToString("N"),
                Items = [new CartItem { Id = Guid.NewGuid(), ProductId = productId, Quantity = 1, UnitPrice = price }]
            }));
            await setup.SaveChangesAsync(cancellationToken);
        }

        var request = new CheckoutRequest("Benchmark Customer", "1 Benchmark Street", "Synthetic District", "90000000", "Benchmark City", "52998224725", dataset.StateId, null);
        var stopwatch = Stopwatch.StartNew();
        var tasks = buyers.Select(async userId =>
        {
            await using var db = CreateDbContext();
            var handler = new CheckoutHandler(db);
            var result = await handler.HandleAsync(request, CreateHttpContext(userId, "Customer"), cancellationToken);
            return (result as IStatusCodeHttpResult)?.StatusCode ?? StatusCodes.Status500InternalServerError;
        });
        var statusCodes = await Task.WhenAll(tasks);
        stopwatch.Stop();

        await using var verify = CreateDbContext();
        var finalStock = await verify.Products.Where(item => item.Id == productId).Select(item => item.Stock).SingleAsync(cancellationToken);
        var created = statusCodes.Count(status => status == StatusCodes.Status201Created);
        var expectedRejected = statusCodes.Count(status => status == StatusCodes.Status400BadRequest);
        if (created != stock || expectedRejected != attempts - stock || finalStock != 0 || statusCodes.Any(status => status is not StatusCodes.Status201Created and not StatusCodes.Status400BadRequest))
        {
            throw new InvalidOperationException("Checkout concorrente violou a consistencia esperada: criados=" + created + ", rejeitados=" + expectedRejected + ", estoque=" + finalStock + ", status=" + string.Join(',', statusCodes));
        }

        return new CheckoutConsistencyResult(attempts, created, expectedRejected, finalStock, stopwatch.Elapsed.TotalMilliseconds);
    }
}
