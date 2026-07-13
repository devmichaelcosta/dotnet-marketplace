using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Produto.ProductImports.SearchJobs;
using Marketplace.Api.Features.Admin.Produto.ProductImports.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using NPOI.XSSF.UserModel;

namespace Marketplace.Tests;

public sealed class ProductImportTests
{
    [Fact]
    public void Product_import_template_can_be_read_with_dynamic_attributes()
    {
        var bytes = ProductImportTemplate.Create();

        var rows = ProductImportWorkbook.ReadRows(bytes);

        Assert.NotEmpty(rows);
        Assert.Contains(rows, row => row.Sku == "IMP-LIVRO-CLEAN-CODE");
        var cleanCode = rows.Single(row => row.Sku == "IMP-LIVRO-CLEAN-CODE");
        Assert.Equal("Robert C. Martin", cleanCode.Attributes["Autor"]);
        Assert.NotEmpty(cleanCode.ImageUrls);
    }

    [Fact]
    public void Product_import_parser_rejects_duplicate_attribute_columns()
    {
        var workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet("Produtos");
        var header = sheet.CreateRow(0);
        var headers = new[]
        {
            "Titulo",
            "LoginVendedor",
            "PrecoAVista",
            "Sku",
            "Estoque",
            "EhOferta",
            "Categoria",
            "Subcategoria",
            "Descritivo",
            "Imagens",
            "Atributo: Marca",
            "Atributo:  marca "
        };

        for (var index = 0; index < headers.Length; index++)
        {
            header.CreateCell(index).SetCellValue(headers[index]);
        }

        using var stream = new MemoryStream();
        workbook.Write(stream, true);

        var exception = Assert.Throws<ProductImportException>(() => ProductImportWorkbook.ReadRows(stream.ToArray()));
        Assert.Contains("Atributos duplicados", exception.Message);
    }

    [Theory]
    [InlineData("SKU ABC 123", "sku-abc-123")]
    [InlineData("IMP/LIVRO:CLEAN", "imp-livro-clean")]
    public void Product_import_image_slug_is_safe_for_file_names(string value, string expected)
    {
        Assert.Equal(expected, ProductImportImageDownloader.Slug(value));
    }

    [Fact]
    public void Product_import_file_path_cannot_escape_webroot()
    {
        var webRoot = Path.GetFullPath(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
        Directory.CreateDirectory(webRoot);

        try
        {
            Assert.Throws<InvalidOperationException>(() => ProductImportFiles.ToAbsolutePath(webRoot, "/../outside.xlsx"));
        }
        finally
        {
            Directory.Delete(webRoot, true);
        }
    }

    [Fact]
    public async Task Product_import_query_supports_search_sort_and_pagination()
    {
        var databaseName = $"DotNetMarketplace_ProductImportQuery_{Guid.NewGuid():N}";
        var connectionString = $"Server=(localdb)\\SGPLocalDB;Database={databaseName};Trusted_Connection=True;TrustServerCertificate=True";
        var options = new DbContextOptionsBuilder<MarketplaceDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        try
        {
            await using (var setup = new MarketplaceDbContext(options))
            {
                await setup.Database.EnsureCreatedAsync();
                setup.ProductImportJobs.AddRange(
                    new ProductImportJob
                    {
                        OriginalFileName = "alpha.xlsx",
                        StoredFileName = "original.xlsx",
                        StoredFilePath = "/uploads/product-imports/alpha/original.xlsx",
                        ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        ImportedByName = "michael",
                        Status = ProductImportJobStatus.Success,
                        SkuCount = 5,
                        CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-5)
                    },
                    new ProductImportJob
                    {
                        OriginalFileName = "beta.xlsx",
                        StoredFileName = "original.xlsx",
                        StoredFilePath = "/uploads/product-imports/beta/original.xlsx",
                        ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        ImportedByName = "admin",
                        Status = ProductImportJobStatus.Failed,
                        SkuCount = 12,
                        ErrorCount = 2,
                        CreatedAt = DateTimeOffset.UtcNow
                    });
                await setup.SaveChangesAsync();
            }

            await using var db = new MarketplaceDbContext(options);
            var handler = new SearchProductImportJobsHandler(db);
            var result = await handler.HandleAsync(new SearchProductImportJobsQuery
            {
                Search = "beta",
                Sort = "skus",
                Direction = "desc",
                Page = 1,
                PageSize = 10
            }, CancellationToken.None);

            Assert.Equal(1, result.Total);
            Assert.Equal("beta.xlsx", result.Items.Single().OriginalFileName);
            Assert.Equal(12, result.Items.Single().SkuCount);
        }
        finally
        {
            await using var cleanup = new MarketplaceDbContext(options);
            await cleanup.Database.EnsureDeletedAsync();
        }
    }
}

