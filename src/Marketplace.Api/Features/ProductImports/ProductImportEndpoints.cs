using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Marketplace.Api.Domain;
using Marketplace.Api.Features;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SkiaSharp;

namespace Marketplace.Api.Features.ProductImports;

public static class ProductImportEndpoints
{
    private const long MaxExcelSizeBytes = 5 * 1024 * 1024;

    public static IEndpointRouteBuilder MapProductImportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/product-imports")
            .RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole))
            .WithTags("Product imports");

        group.MapGet("/template", () =>
        {
            var bytes = ProductImportTemplate.Create();
            return Results.File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "product-import-template.xlsx");
        });

        group.MapPost("/", async Task<Results<Accepted<ProductImportCreatedResponse>, ValidationProblem, UnauthorizedHttpResult>> (
            [FromForm] IFormFile file,
            HttpContext http,
            MarketplaceDbContext db,
            IWebHostEnvironment environment,
            ProductImportQueue queue,
            CancellationToken cancellationToken) =>
        {
            var userId = http.User.GetUserId();
            if (userId is null)
            {
                return TypedResults.Unauthorized();
            }

            var errors = ValidateUpload(file);
            if (errors.Count > 0)
            {
                return TypedResults.ValidationProblem(errors);
            }

            var job = new ProductImportJob
            {
                OriginalFileName = Path.GetFileName(file.FileName),
                StoredFileName = "original.xlsx",
                ContentType = file.ContentType,
                FileSizeBytes = file.Length,
                ImportedByUserId = userId.Value,
                ImportedByName = http.User.Identity?.Name ?? "admin",
                Status = ProductImportJobStatus.Pending,
                SummaryMessage = "Importacao aguardando processamento."
            };

            db.ProductImportJobs.Add(job);
            await db.SaveChangesAsync(cancellationToken);

            var relativeDirectory = Path.Combine("uploads", "product-imports", DateTimeOffset.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture), job.Id.ToString(CultureInfo.InvariantCulture));
            var absoluteDirectory = Path.Combine(environment.WebRootPath, relativeDirectory);
            Directory.CreateDirectory(absoluteDirectory);
            var absolutePath = Path.Combine(absoluteDirectory, job.StoredFileName);
            await using (var stream = File.Create(absolutePath))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            job.StoredFilePath = "/" + Path.Combine(relativeDirectory, job.StoredFileName).Replace('\\', '/');
            await db.SaveChangesAsync(cancellationToken);
            await queue.EnqueueAsync(job.Id, cancellationToken);

            return TypedResults.Accepted($"/api/admin/product-imports/{job.Id}", new ProductImportCreatedResponse(job.Id));
        })
        .DisableAntiforgery();

        group.MapGet("/", async (
            string? search,
            string? status,
            string? sort,
            string? direction,
            int page,
            int pageSize,
            MarketplaceDbContext db,
            CancellationToken cancellationToken) =>
        {
            var result = await ProductImportQueries.SearchJobsAsync(db, search, status, sort, direction, page, pageSize, cancellationToken);
            return Results.Ok(result);
        });

        group.MapGet("/{id:int}", async (int id, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var job = await db.ProductImportJobs.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
            return job is null ? Results.NotFound() : Results.Ok(ProductImportJobDetails.From(job));
        });

        group.MapGet("/{id:int}/items", async (
            int id,
            string? search,
            string? status,
            string? sort,
            string? direction,
            int page,
            int pageSize,
            MarketplaceDbContext db,
            CancellationToken cancellationToken) =>
        {
            if (!await db.ProductImportJobs.AnyAsync(item => item.Id == id, cancellationToken))
            {
                return Results.NotFound();
            }

            var result = await ProductImportQueries.SearchItemsAsync(db, id, search, status, sort, direction, page, pageSize, cancellationToken);
            return Results.Ok(result);
        });

        group.MapGet("/{id:int}/file", async (int id, MarketplaceDbContext db, IWebHostEnvironment environment, CancellationToken cancellationToken) =>
        {
            var job = await db.ProductImportJobs.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
            if (job is null)
            {
                return Results.NotFound();
            }

            var absolutePath = ProductImportFiles.ToAbsolutePath(environment.WebRootPath, job.StoredFilePath);
            if (!File.Exists(absolutePath))
            {
                return Results.NotFound();
            }

            var bytes = await File.ReadAllBytesAsync(absolutePath, cancellationToken);
            return Results.File(bytes, job.ContentType, job.OriginalFileName);
        });

        return app;
    }

    private static Dictionary<string, string[]> ValidateUpload(IFormFile file)
    {
        var errors = new Dictionary<string, string[]>();
        if (file.Length <= 0)
        {
            errors["file"] = ["Arquivo obrigatorio."];
        }
        else if (file.Length > MaxExcelSizeBytes)
        {
            errors["file"] = ["Arquivo deve ter no maximo 5 MB."];
        }

        var extension = Path.GetExtension(file.FileName);
        if (!extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase) && !extension.Equals(".xls", StringComparison.OrdinalIgnoreCase))
        {
            errors["fileType"] = ["Envie uma planilha Excel .xlsx ou .xls."];
        }

        return errors;
    }
}

public sealed class ProductImportQueue
{
    private readonly Channel<int> channel = Channel.CreateUnbounded<int>();
    private readonly ConcurrentDictionary<int, byte> queued = [];

    public async ValueTask EnqueueAsync(int jobId, CancellationToken cancellationToken = default)
    {
        if (!queued.TryAdd(jobId, 0))
        {
            return;
        }

        await channel.Writer.WriteAsync(jobId, cancellationToken);
    }

    public async ValueTask<int> DequeueAsync(CancellationToken cancellationToken)
    {
        var jobId = await channel.Reader.ReadAsync(cancellationToken);
        queued.TryRemove(jobId, out _);
        return jobId;
    }
}

public sealed class ProductImportWorker(
    IServiceScopeFactory scopeFactory,
    ProductImportQueue queue,
    ILogger<ProductImportWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RequeuePendingJobsAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var jobId = await queue.DequeueAsync(stoppingToken);
            try
            {
                using var scope = scopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<ProductImportProcessor>();
                await processor.ProcessAsync(jobId, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected failure while processing product import job {JobId}.", jobId);
            }
        }
    }

    private async Task RequeuePendingJobsAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MarketplaceDbContext>();
        var jobs = await db.ProductImportJobs
            .Where(job => job.Status == ProductImportJobStatus.Pending || job.Status == ProductImportJobStatus.Processing)
            .ToListAsync(cancellationToken);

        foreach (var job in jobs)
        {
            if (job.Status == ProductImportJobStatus.Processing)
            {
                job.Status = ProductImportJobStatus.Pending;
                job.StartedAt = null;
                job.SummaryMessage = "Retomado apos reinicio.";
            }

            await queue.EnqueueAsync(job.Id, cancellationToken);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}

public sealed class ProductImportProcessor(
    MarketplaceDbContext db,
    IWebHostEnvironment environment,
    ProductImportImageDownloader imageDownloader,
    ILogger<ProductImportProcessor> logger)
{
    public async Task ProcessAsync(int jobId, CancellationToken cancellationToken)
    {
        var job = await db.ProductImportJobs.Include(item => item.Items).FirstOrDefaultAsync(item => item.Id == jobId, cancellationToken);
        if (job is null)
        {
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        job.Status = ProductImportJobStatus.Processing;
        job.StartedAt = DateTimeOffset.UtcNow;
        job.FinishedAt = null;
        job.DurationMs = null;
        job.SummaryMessage = "Importacao em processamento.";
        db.ProductImportJobItems.RemoveRange(job.Items);
        await db.SaveChangesAsync(cancellationToken);

        var downloadedFiles = new List<string>();
        try
        {
            var absolutePath = ProductImportFiles.ToAbsolutePath(environment.WebRootPath, job.StoredFilePath);
            var rows = ProductImportWorkbook.ReadRows(await File.ReadAllBytesAsync(absolutePath, cancellationToken));
            var validation = await ValidateRowsAsync(rows, cancellationToken);

            if (validation.ErrorItems.Count > 0)
            {
                CompleteFailed(job, stopwatch, rows, validation.ErrorItems, "Importacao falhou. Corrija os erros da planilha e envie novamente.");
                await db.SaveChangesAsync(cancellationToken);
                return;
            }

            foreach (var row in rows)
            {
                var images = await Task.Run(() => imageDownloader.DownloadImagesAsync(row.Sku, row.ImageUrls, cancellationToken), cancellationToken)
                    .WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);
                row.DownloadedImages.AddRange(images.RelativePaths);
                downloadedFiles.AddRange(images.AbsolutePaths);
            }

            await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
            var resultItems = await UpsertProductsAsync(rows, validation, cancellationToken);
            db.ProductImportJobItems.AddRange(resultItems);
            CompleteSuccess(job, stopwatch, rows, resultItems);
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (ProductImportException ex)
        {
            DeleteDownloadedFiles(downloadedFiles);
            logger.LogWarning(ex, "Product import job {JobId} failed.", job.Id);
            CompleteFailed(job, stopwatch, [], [new ProductImportJobItem
            {
                RowNumber = ex.RowNumber,
                Sku = ex.Sku,
                Title = ex.Title,
                Status = ProductImportJobItemStatus.Error,
                ErrorMessage = ex.Message
            }], "Importacao falhou durante o processamento.");
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            DeleteDownloadedFiles(downloadedFiles);
            logger.LogError(ex, "Product import job {JobId} failed.", job.Id);
            CompleteFailed(job, stopwatch, [], [new ProductImportJobItem
            {
                Status = ProductImportJobItemStatus.Error,
                ErrorMessage = "Erro inesperado durante a importacao."
            }], "Importacao falhou durante o processamento.");
            await db.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<ProductImportValidation> ValidateRowsAsync(IReadOnlyList<ProductImportRow> rows, CancellationToken cancellationToken)
    {
        var result = new ProductImportValidation();
        if (rows.Count == 0)
        {
            result.ErrorItems.Add(ErrorItem(1, string.Empty, string.Empty, "Planilha nao contem produtos."));
            return result;
        }

        var duplicateSkus = rows
            .Where(row => !string.IsNullOrWhiteSpace(row.Sku))
            .GroupBy(row => ProductImportWorkbook.NormalizeKey(row.Sku))
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var sellerLogins = rows.Select(row => row.LoginVendedor).Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var users = await db.Users.Where(user => sellerLogins.Contains(user.UserName!)).ToListAsync(cancellationToken);
        var sellers = await db.Sellers.ToListAsync(cancellationToken);
        var sellerUserIds = sellers.Select(seller => seller.UserId).ToHashSet();
        result.SellersByLogin = users
            .Where(user => user.UserName is not null && sellerUserIds.Contains(user.Id))
            .ToDictionary(user => user.UserName!, user => user, StringComparer.OrdinalIgnoreCase);

        var categories = await db.Categories.Include(item => item.SubCategories).ToListAsync(cancellationToken);
        foreach (var category in categories)
        {
            result.CategoriesByName[ProductImportWorkbook.NormalizeKey(category.Title)] = category;
        }

        foreach (var row in rows)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(row.Title))
            {
                errors.Add("Titulo obrigatorio.");
            }

            if (string.IsNullOrWhiteSpace(row.Sku))
            {
                errors.Add("SKU obrigatorio.");
            }
            else if (duplicateSkus.Contains(ProductImportWorkbook.NormalizeKey(row.Sku)))
            {
                errors.Add("SKU duplicado na planilha.");
            }

            if (row.Price <= 0)
            {
                errors.Add("PrecoAVista deve ser maior que zero.");
            }

            if (row.Stock < 0)
            {
                errors.Add("Estoque nao pode ser negativo.");
            }

            if (!result.SellersByLogin.ContainsKey(row.LoginVendedor))
            {
                errors.Add("LoginVendedor nao encontrado ou nao pertence a um vendedor.");
            }

            if (!result.CategoriesByName.TryGetValue(ProductImportWorkbook.NormalizeKey(row.Category), out var category))
            {
                errors.Add("Categoria nao encontrada.");
            }
            else
            {
                var subCategory = category.SubCategories.FirstOrDefault(item => ProductImportWorkbook.NormalizeKey(item.Title) == ProductImportWorkbook.NormalizeKey(row.SubCategory));
                if (subCategory is null)
                {
                    errors.Add("Subcategoria nao encontrada para a categoria informada.");
                }
                else
                {
                    result.SubCategoriesByRow[row.RowNumber] = subCategory;
                }
            }

            if (string.IsNullOrWhiteSpace(row.Description))
            {
                errors.Add("Descritivo obrigatorio.");
            }

            if (row.ImageUrls.Count == 0)
            {
                errors.Add("Informe ao menos uma imagem externa.");
            }
            else if (row.ImageUrls.Count > 10)
            {
                errors.Add("Informe no maximo 10 imagens por produto.");
            }

            foreach (var url in row.ImageUrls)
            {
                if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                {
                    errors.Add($"Imagem invalida: {url}");
                }
            }

            if (errors.Count > 0)
            {
                result.ErrorItems.Add(ErrorItem(row.RowNumber, row.Sku, row.Title, string.Join(" ", errors)));
            }
        }

        return result;
    }

    private async Task<List<ProductImportJobItem>> UpsertProductsAsync(IReadOnlyList<ProductImportRow> rows, ProductImportValidation validation, CancellationToken cancellationToken)
    {
        var normalizedAttributeNames = rows.SelectMany(row => row.Attributes.Keys)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var existingAttributes = await db.Attributes.ToListAsync(cancellationToken);
        var attributesByName = existingAttributes.ToDictionary(item => ProductImportWorkbook.NormalizeKey(item.Name), item => item);

        foreach (var name in normalizedAttributeNames)
        {
            var normalized = ProductImportWorkbook.NormalizeKey(name);
            if (!attributesByName.ContainsKey(normalized))
            {
                var attribute = new AttributeDefinition { Name = name };
                db.Attributes.Add(attribute);
                attributesByName[normalized] = attribute;
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        var skus = rows.Select(row => row.Sku).ToArray();
        var products = await db.Products
            .Include(item => item.Images)
            .Include(item => item.AttributeValues)
            .Where(item => skus.Contains(item.Sku))
            .ToListAsync(cancellationToken);
        var productsBySku = products.ToDictionary(item => item.Sku, item => item, StringComparer.OrdinalIgnoreCase);
        var items = new List<ProductImportJobItem>();

        foreach (var row in rows)
        {
            var exists = productsBySku.TryGetValue(row.Sku, out var product);
            if (!exists)
            {
                product = new Product { CreatedAt = DateTimeOffset.UtcNow, CreatedBy = "product-import" };
                db.Products.Add(product);
            }
            else
            {
                db.ProductImages.RemoveRange(product!.Images);
                db.ProductAttributeValues.RemoveRange(product.AttributeValues);
            }

            product!.UserId = validation.SellersByLogin[row.LoginVendedor].Id;
            product.SubCategoryId = validation.SubCategoriesByRow[row.RowNumber].Id;
            product.Title = row.Title;
            product.Description = row.Description;
            product.Price = row.Price;
            product.Stock = row.Stock;
            product.Offer = row.Offer;
            product.Sku = row.Sku;
            product.Images = row.DownloadedImages.Select(image => new ProductImage { FileName = image }).ToList();
            product.AttributeValues = row.Attributes.Select(attribute => new ProductAttributeValue
            {
                AttributeDefinitionId = attributesByName[ProductImportWorkbook.NormalizeKey(attribute.Key)].Id,
                Value = attribute.Value
            }).ToList();

            items.Add(new ProductImportJobItem
            {
                RowNumber = row.RowNumber,
                Sku = row.Sku,
                Title = row.Title,
                Status = exists ? ProductImportJobItemStatus.Updated : ProductImportJobItemStatus.Created,
                ProductId = product.Id == 0 ? null : product.Id,
                DownloadedImages = JsonSerializer.Serialize(row.DownloadedImages),
                ImportedAttributes = JsonSerializer.Serialize(row.Attributes)
            });
        }

        await db.SaveChangesAsync(cancellationToken);
        var savedProductsBySku = await db.Products
            .Where(product => skus.Contains(product.Sku))
            .ToDictionaryAsync(product => product.Sku, product => product.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);
        foreach (var item in items)
        {
            item.ProductId = savedProductsBySku[item.Sku];
        }

        return items;
    }

    private static ProductImportJobItem ErrorItem(int rowNumber, string sku, string title, string message) =>
        new()
        {
            RowNumber = rowNumber,
            Sku = sku,
            Title = title,
            Status = ProductImportJobItemStatus.Error,
            ErrorMessage = message
        };

    private static void CompleteSuccess(ProductImportJob job, Stopwatch stopwatch, IReadOnlyList<ProductImportRow> rows, IReadOnlyList<ProductImportJobItem> items)
    {
        stopwatch.Stop();
        job.Status = ProductImportJobStatus.Success;
        job.FinishedAt = DateTimeOffset.UtcNow;
        job.DurationMs = stopwatch.ElapsedMilliseconds;
        job.TotalRows = rows.Count;
        job.SkuCount = rows.Select(row => row.Sku).Distinct(StringComparer.OrdinalIgnoreCase).Count();
        job.CreatedCount = items.Count(item => item.Status == ProductImportJobItemStatus.Created);
        job.UpdatedCount = items.Count(item => item.Status == ProductImportJobItemStatus.Updated);
        job.ErrorCount = 0;
        job.SummaryMessage = "Importacao concluida com sucesso.";
    }

    private void CompleteFailed(ProductImportJob job, Stopwatch stopwatch, IReadOnlyList<ProductImportRow> rows, IReadOnlyList<ProductImportJobItem> items, string message)
    {
        stopwatch.Stop();
        job.Status = ProductImportJobStatus.Failed;
        job.FinishedAt = DateTimeOffset.UtcNow;
        job.DurationMs = stopwatch.ElapsedMilliseconds;
        job.TotalRows = rows.Count;
        job.SkuCount = rows.Where(row => !string.IsNullOrWhiteSpace(row.Sku)).Select(row => row.Sku).Distinct(StringComparer.OrdinalIgnoreCase).Count();
        job.CreatedCount = 0;
        job.UpdatedCount = 0;
        job.ErrorCount = items.Count(item => item.Status == ProductImportJobItemStatus.Error);
        job.SummaryMessage = message;
        db.ProductImportJobItems.AddRange(items);
    }

    private static void DeleteDownloadedFiles(IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }
}

public sealed class ProductImportImageDownloader(IHttpClientFactory httpClientFactory, IWebHostEnvironment environment)
{
    private const long MaxImageSizeBytes = 8 * 1024 * 1024;

    public async Task<ProductImportDownloadedImages> DownloadImagesAsync(string sku, IReadOnlyList<string> imageUrls, CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient("product-import-images");
        var dateFolder = DateTimeOffset.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        var relativeDirectory = Path.Combine("uploads", "products", "imports", dateFolder);
        var absoluteDirectory = Path.Combine(environment.WebRootPath, relativeDirectory);
        Directory.CreateDirectory(absoluteDirectory);

        var result = new ProductImportDownloadedImages();
        for (var index = 0; index < imageUrls.Count; index++)
        {
            var url = imageUrls[index];
            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new ProductImportException($"Nao foi possivel baixar a imagem {url}.");
            }

            await using var source = await response.Content.ReadAsStreamAsync(cancellationToken);
            await using var limited = new MemoryStream();
            await CopyWithLimitAsync(source, limited, MaxImageSizeBytes, cancellationToken);
            limited.Position = 0;

            using var bitmap = SKBitmap.Decode(limited);
            if (bitmap is null)
            {
                throw new ProductImportException($"URL nao retornou uma imagem valida: {url}.");
            }

            using var image = SKImage.FromBitmap(bitmap);
            using var encoded = image.Encode(SKEncodedImageFormat.Jpeg, 88);
            var fileName = $"{Slug(sku)}-{index + 1}-{Hash(url)}.jpg";
            var absolutePath = Path.Combine(absoluteDirectory, fileName);
            await using (var output = File.Create(absolutePath))
            {
                encoded.SaveTo(output);
            }

            result.AbsolutePaths.Add(absolutePath);
            result.RelativePaths.Add("/" + Path.Combine(relativeDirectory, fileName).Replace('\\', '/'));
        }

        return result;
    }

    private static async Task CopyWithLimitAsync(Stream source, Stream destination, long limit, CancellationToken cancellationToken)
    {
        var buffer = new byte[81920];
        long total = 0;
        int read;
        while ((read = await source.ReadAsync(buffer, cancellationToken)) > 0)
        {
            total += read;
            if (total > limit)
            {
                throw new ProductImportException("Imagem excede o limite de 8 MB.");
            }

            await destination.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
        }
    }

    public static string Slug(string value)
    {
        var builder = new StringBuilder();
        foreach (var character in value.ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(character);
            }
            else if (builder.Length == 0 || builder[^1] != '-')
            {
                builder.Append('-');
            }
        }

        return builder.ToString().Trim('-');
    }

    private static string Hash(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant()[..8];
}

public static class ProductImportWorkbook
{
    private static readonly string[] RequiredHeaders =
    [
        "Titulo",
        "LoginVendedor",
        "PrecoAVista",
        "Sku",
        "Estoque",
        "EhOferta",
        "Categoria",
        "Subcategoria",
        "Descritivo",
        "Imagens"
    ];

    public static List<ProductImportRow> ReadRows(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        var workbook = WorkbookFactory.Create(stream);
        var sheet = workbook.GetSheet("Produtos") ?? workbook.GetSheetAt(0);
        var headerRow = sheet.GetRow(sheet.FirstRowNum) ?? throw new ProductImportException("Planilha sem cabecalho.");
        var headers = ReadHeaders(headerRow);
        ValidateHeaders(headers);

        var rows = new List<ProductImportRow>();
        var formatter = new DataFormatter(CultureInfo.GetCultureInfo("pt-BR"));
        for (var rowIndex = sheet.FirstRowNum + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
        {
            var row = sheet.GetRow(rowIndex);
            if (row is null || IsEmpty(row, formatter, headers.Count))
            {
                continue;
            }

            rows.Add(ReadRow(row, formatter, headers));
        }

        return rows;
    }

    public static string NormalizeKey(string value) =>
        string.Join(' ', value.Trim().ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries));

    private static ProductImportRow ReadRow(IRow row, DataFormatter formatter, IReadOnlyList<ProductImportHeader> headers)
    {
        string Cell(string name) => formatter.FormatCellValue(row.GetCell(headers.First(header => header.Name == name).Index)).Trim();
        var parsed = new ProductImportRow
        {
            RowNumber = row.RowNum + 1,
            Title = Cell("Titulo"),
            LoginVendedor = Cell("LoginVendedor"),
            Sku = Cell("Sku"),
            Category = Cell("Categoria"),
            SubCategory = Cell("Subcategoria"),
            Description = Cell("Descritivo"),
            ImageUrls = Cell("Imagens").Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList()
        };

        parsed.Price = ParseDecimal(Cell("PrecoAVista"), parsed.RowNumber);
        parsed.Stock = ParseInt(Cell("Estoque"), parsed.RowNumber, "Estoque");
        parsed.Offer = ParseBool(Cell("EhOferta"), parsed.RowNumber);

        foreach (var header in headers.Where(header => header.AttributeName is not null))
        {
            var value = formatter.FormatCellValue(row.GetCell(header.Index)).Trim();
            if (!string.IsNullOrWhiteSpace(value))
            {
                parsed.Attributes[header.AttributeName!] = value;
            }
        }

        return parsed;
    }

    private static List<ProductImportHeader> ReadHeaders(IRow headerRow)
    {
        var headers = new List<ProductImportHeader>();
        for (var index = 0; index < headerRow.LastCellNum; index++)
        {
            var value = headerRow.GetCell(index)?.StringCellValue?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            var canonical = CanonicalHeader(value);
            var attributeName = value.StartsWith("Atributo:", StringComparison.OrdinalIgnoreCase)
                ? value["Atributo:".Length..].Trim()
                : null;
            headers.Add(new ProductImportHeader(index, canonical, attributeName));
        }

        return headers;
    }

    private static void ValidateHeaders(IReadOnlyList<ProductImportHeader> headers)
    {
        var names = headers.Select(header => header.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missing = RequiredHeaders.Where(header => !names.Contains(header)).ToArray();
        if (missing.Length > 0)
        {
            throw new ProductImportException($"Cabecalhos obrigatorios ausentes: {string.Join(", ", missing)}.");
        }

        var duplicateAttributes = headers
            .Where(header => header.AttributeName is not null)
            .GroupBy(header => NormalizeKey(header.AttributeName!))
            .Where(group => group.Count() > 1)
            .Select(group => group.First().AttributeName)
            .ToArray();
        if (duplicateAttributes.Length > 0)
        {
            throw new ProductImportException($"Atributos duplicados: {string.Join(", ", duplicateAttributes)}.");
        }
    }

    private static string CanonicalHeader(string value)
    {
        var normalized = NormalizeKey(value).Replace(" ", string.Empty, StringComparison.Ordinal);
        return normalized switch
        {
            "titulo" => "Titulo",
            "loginvendedor" => "LoginVendedor",
            "precoavista" or "precoavista" => "PrecoAVista",
            "sku" => "Sku",
            "estoque" => "Estoque",
            "ehoferta" or "eoferta" => "EhOferta",
            "categoria" => "Categoria",
            "subcategoria" => "Subcategoria",
            "descritivo" or "descricao" => "Descritivo",
            "imagens" => "Imagens",
            _ when value.StartsWith("Atributo:", StringComparison.OrdinalIgnoreCase) => value,
            _ => value
        };
    }

    private static bool IsEmpty(IRow row, DataFormatter formatter, int cellCount)
    {
        for (var index = 0; index < cellCount; index++)
        {
            if (!string.IsNullOrWhiteSpace(formatter.FormatCellValue(row.GetCell(index))))
            {
                return false;
            }
        }

        return true;
    }

    private static decimal ParseDecimal(string value, int rowNumber)
    {
        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.GetCultureInfo("pt-BR"), out var ptValue) ||
            decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out ptValue))
        {
            return ptValue;
        }

        throw new ProductImportException("PrecoAVista invalido.", rowNumber);
    }

    private static int ParseInt(string value, int rowNumber, string field)
    {
        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        throw new ProductImportException($"{field} invalido.", rowNumber);
    }

    private static bool ParseBool(string value, int rowNumber)
    {
        return NormalizeKey(value) switch
        {
            "sim" or "true" or "1" or "s" => true,
            "nao" or "não" or "false" or "0" or "n" => false,
            _ => throw new ProductImportException("EhOferta invalido.", rowNumber)
        };
    }
}

public static class ProductImportTemplate
{
    public static byte[] Create()
    {
        IWorkbook workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet("Produtos");
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
            "Atributo: Modelo",
            "Atributo: Autor",
            "Atributo: Editora",
            "Atributo: Processador",
            "Atributo: Memoria RAM",
            "Atributo: Volume",
            "Atributo: Cordas"
        };
        var headerRow = sheet.CreateRow(0);
        for (var index = 0; index < headers.Length; index++)
        {
            headerRow.CreateCell(index).SetCellValue(headers[index]);
            sheet.SetColumnWidth(index, 22 * 256);
        }

        var examples = new[]
        {
            new[] { "Codigo Limpo", "techstore", "79,90", "IMP-LIVRO-CLEAN-CODE", "15", "sim", "Livros", "Tecnologia", "Boas praticas para escrever codigo legivel, testavel e sustentavel.", "https://placehold.co/900x900.jpg?text=Codigo+Limpo", "Alta Books", "", "Robert C. Martin", "Alta Books", "", "", "", "" },
            new[] { "Notebook Acer Predator Helios 300", "techstore", "7499,00", "IMP-NOTE-ACER-PREDATOR", "4", "sim", "Informatica", "Notebooks", "Notebook gamer com alto desempenho para jogos, desenvolvimento e criacao.", "https://placehold.co/900x900.jpg?text=Acer+Predator", "Acer", "Predator Helios 300", "", "", "Intel Core i7", "16 GB", "", "" },
            new[] { "Violao Tagima Dallas Tuner Eletroacustico", "multisom", "899,90", "IMP-VIOL-TAGIMA-DALLAS", "11", "sim", "Instrumentos musicais", "Violoes", "Violao com afinador embutido, cordas de aco e otima resposta para estudo e palco.", "https://placehold.co/900x900.jpg?text=Tagima+Dallas", "Tagima", "Dallas Tuner", "", "", "", "", "", "Aco" },
            new[] { "Coca-Cola Zero Acucar 350ml", "techstore", "4,99", "IMP-BEB-COCA-ZERO-350", "120", "nao", "Bebidas", "Refrigerantes", "Refrigerante zero acucar em lata de 350 ml.", "https://placehold.co/900x900.jpg?text=Coca-Cola+Zero", "Coca-Cola", "", "", "", "", "", "350 ml", "" }
        };

        for (var rowIndex = 0; rowIndex < examples.Length; rowIndex++)
        {
            var row = sheet.CreateRow(rowIndex + 1);
            for (var cellIndex = 0; cellIndex < examples[rowIndex].Length; cellIndex++)
            {
                row.CreateCell(cellIndex).SetCellValue(examples[rowIndex][cellIndex]);
            }
        }

        using var stream = new MemoryStream();
        workbook.Write(stream, true);
        return stream.ToArray();
    }
}

public static class ProductImportQueries
{
    public static async Task<PagedResult<ProductImportJobListItem>> SearchJobsAsync(
        MarketplaceDbContext db,
        string? search,
        string? status,
        string? sort,
        string? direction,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize <= 0 ? 10 : pageSize, 1, 100);
        var query = db.ProductImportJobs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(job => job.OriginalFileName.Contains(search) || job.ImportedByName.Contains(search) || job.SummaryMessage.Contains(search));
        }

        if (Enum.TryParse<ProductImportJobStatus>(status, true, out var parsedStatus))
        {
            query = query.Where(job => job.Status == parsedStatus);
        }

        query = (sort?.ToLowerInvariant(), direction?.Equals("asc", StringComparison.OrdinalIgnoreCase) == true) switch
        {
            ("status", true) => query.OrderBy(job => job.Status),
            ("status", false) => query.OrderByDescending(job => job.Status),
            ("user", true) => query.OrderBy(job => job.ImportedByName),
            ("user", false) => query.OrderByDescending(job => job.ImportedByName),
            ("duration", true) => query.OrderBy(job => job.DurationMs),
            ("duration", false) => query.OrderByDescending(job => job.DurationMs),
            ("skus", true) => query.OrderBy(job => job.SkuCount),
            ("skus", false) => query.OrderByDescending(job => job.SkuCount),
            ("errors", true) => query.OrderBy(job => job.ErrorCount),
            ("errors", false) => query.OrderByDescending(job => job.ErrorCount),
            ("created", true) => query.OrderBy(job => job.CreatedAt),
            _ => query.OrderByDescending(job => job.CreatedAt)
        };

        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).Select(job => ProductImportJobListItem.From(job)).ToListAsync(cancellationToken);
        return new PagedResult<ProductImportJobListItem>(items, total, page, pageSize);
    }

    public static async Task<PagedResult<ProductImportJobItemResponse>> SearchItemsAsync(
        MarketplaceDbContext db,
        int jobId,
        string? search,
        string? status,
        string? sort,
        string? direction,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize <= 0 ? 10 : pageSize, 1, 100);
        var query = db.ProductImportJobItems.AsNoTracking().Where(item => item.JobId == jobId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item => item.Sku.Contains(search) || item.Title.Contains(search) || item.ErrorMessage.Contains(search));
        }

        if (Enum.TryParse<ProductImportJobItemStatus>(status, true, out var parsedStatus))
        {
            query = query.Where(item => item.Status == parsedStatus);
        }

        query = (sort?.ToLowerInvariant(), direction?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true) switch
        {
            ("sku", true) => query.OrderByDescending(item => item.Sku),
            ("sku", false) => query.OrderBy(item => item.Sku),
            ("status", true) => query.OrderByDescending(item => item.Status),
            ("status", false) => query.OrderBy(item => item.Status),
            ("row", true) => query.OrderByDescending(item => item.RowNumber),
            _ => query.OrderBy(item => item.RowNumber)
        };

        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).Select(item => ProductImportJobItemResponse.From(item)).ToListAsync(cancellationToken);
        return new PagedResult<ProductImportJobItemResponse>(items, total, page, pageSize);
    }
}

public static class ProductImportFiles
{
    public static string ToAbsolutePath(string webRootPath, string storedFilePath)
    {
        var relative = storedFilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var absolute = Path.GetFullPath(Path.Combine(webRootPath, relative));
        var root = Path.GetFullPath(webRootPath);
        if (!absolute.StartsWith(root, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Caminho de arquivo invalido.");
        }

        return absolute;
    }
}

public sealed class ProductImportException : Exception
{
    public ProductImportException(string message, int rowNumber = 0, string sku = "", string title = "") : base(message)
    {
        RowNumber = rowNumber;
        Sku = sku;
        Title = title;
    }

    public int RowNumber { get; }
    public string Sku { get; }
    public string Title { get; }
}

public sealed record ProductImportCreatedResponse(int JobId);
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);
public sealed record ProductImportJobListItem(
    int Id,
    string OriginalFileName,
    string ImportedByName,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? FinishedAt,
    long? DurationMs,
    int TotalRows,
    int SkuCount,
    int CreatedCount,
    int UpdatedCount,
    int ErrorCount,
    string SummaryMessage)
{
    public static ProductImportJobListItem From(ProductImportJob job) =>
        new(
            job.Id,
            job.OriginalFileName,
            job.ImportedByName,
            job.Status.ToString(),
            job.CreatedAt,
            job.StartedAt,
            job.FinishedAt,
            job.DurationMs,
            job.TotalRows,
            job.SkuCount,
            job.CreatedCount,
            job.UpdatedCount,
            job.ErrorCount,
            job.SummaryMessage);
}

public sealed record ProductImportJobDetails(
    int Id,
    string OriginalFileName,
    string ImportedByName,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? FinishedAt,
    long? DurationMs,
    int TotalRows,
    int SkuCount,
    int CreatedCount,
    int UpdatedCount,
    int ErrorCount,
    string SummaryMessage,
    long FileSizeBytes)
{
    public static ProductImportJobDetails From(ProductImportJob job) =>
        new(
            job.Id,
            job.OriginalFileName,
            job.ImportedByName,
            job.Status.ToString(),
            job.CreatedAt,
            job.StartedAt,
            job.FinishedAt,
            job.DurationMs,
            job.TotalRows,
            job.SkuCount,
            job.CreatedCount,
            job.UpdatedCount,
            job.ErrorCount,
            job.SummaryMessage,
            job.FileSizeBytes);
}

public sealed record ProductImportJobItemResponse(
    int Id,
    int RowNumber,
    string Sku,
    string Title,
    string Status,
    string ErrorMessage,
    int? ProductId,
    string DownloadedImages,
    string ImportedAttributes)
{
    public static ProductImportJobItemResponse From(ProductImportJobItem item) =>
        new(
            item.Id,
            item.RowNumber,
            item.Sku,
            item.Title,
            item.Status.ToString(),
            item.ErrorMessage,
            item.ProductId,
            item.DownloadedImages,
            item.ImportedAttributes);
}

public sealed class ProductImportRow
{
    public int RowNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string LoginVendedor { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Sku { get; set; } = string.Empty;
    public int Stock { get; set; }
    public bool Offer { get; set; }
    public string Category { get; set; } = string.Empty;
    public string SubCategory { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = [];
    public List<string> DownloadedImages { get; } = [];
    public Dictionary<string, string> Attributes { get; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class ProductImportValidation
{
    public Dictionary<string, ApplicationUser> SellersByLogin { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, Category> CategoriesByName { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<int, SubCategory> SubCategoriesByRow { get; } = [];
    public List<ProductImportJobItem> ErrorItems { get; } = [];
}

public sealed class ProductImportDownloadedImages
{
    public List<string> AbsolutePaths { get; } = [];
    public List<string> RelativePaths { get; } = [];
}

public sealed record ProductImportHeader(int Index, string Name, string? AttributeName);
