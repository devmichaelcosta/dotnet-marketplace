namespace Marketplace.Tests;

public sealed class ApiCorsConfigurationTests
{
    [Fact]
    public void Api_uses_an_explicit_origin_allowlist_with_credentials_for_browser_auth_calls()
    {
        var apiProgram = ReadSource("src", "Marketplace.Api", "Program.cs");
        var webProgram = ReadSource("src", "Marketplace.Web", "Program.cs");

        Assert.Contains("Cors:AllowedOrigins", apiProgram);
        Assert.Contains("WithOrigins(allowedOrigins)", apiProgram);
        Assert.Contains("AllowCredentials()", apiProgram);
        Assert.DoesNotContain("AllowAnyOrigin()", apiProgram);
        Assert.Contains("marketplace-api", webProgram);

        var developmentSettings = ReadSource("src", "Marketplace.Api", "appsettings.Development.json");
        Assert.Contains("http://localhost:5228", developmentSettings);
        Assert.Contains("AddHttpClient<MarketplaceApiClient>", webProgram);
    }

    private static string ReadSource(params string[] relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "DotNetMarketplace.slnx")))
        {
            directory = directory.Parent;
        }

        Assert.NotNull(directory);
        return File.ReadAllText(Path.Combine([directory!.FullName, .. relativePath]));
    }
}
