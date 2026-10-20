using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

var connectionString = Environment.GetEnvironmentVariable("MARKETPLACE_BENCHMARK_CONNECTION")
    ?? "Server=(localdb)\\SGPLocalDB;Database=DotNetMarketplace_Benchmark_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + ";Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
var connection = new SqlConnectionStringBuilder(connectionString);
if (!connection.DataSource.Contains("(localdb)\\", StringComparison.OrdinalIgnoreCase) ||
    !connection.InitialCatalog.StartsWith("DotNetMarketplace_Benchmark_", StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException("O benchmark so pode usar um banco LocalDB cujo nome comece por DotNetMarketplace_Benchmark_.");
}

var options = new DbContextOptionsBuilder<MarketplaceDbContext>()
    .UseSqlServer(connection.ConnectionString)
    .Options;
await using (var db = new MarketplaceDbContext(options))
{
    await db.Database.EnsureDeletedAsync();
    await db.Database.MigrateAsync();
}

var dataset = await BenchmarkDatasetFactory.CreateAsync(options);
var serverVersion = await GetSqlServerVersionAsync(connection.ConnectionString);
var profiler = new WorkloadProfiler(options, dataset);
var result = await profiler.RunAsync(serverVersion, CancellationToken.None);
var outputPath = Path.GetFullPath(Path.Combine(".tmp", "performance-baseline", "results.json"));
Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
await File.WriteAllTextAsync(outputPath, System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
Console.WriteLine("Arquivo de resultados: " + outputPath);

static async Task<string> GetSqlServerVersionAsync(string connectionString)
{
    await using var connection = new SqlConnection(connectionString);
    await connection.OpenAsync();
    await using var command = new SqlCommand("SELECT @@VERSION", connection);
    return Convert.ToString(await command.ExecuteScalarAsync()) ?? "desconhecida";
}
