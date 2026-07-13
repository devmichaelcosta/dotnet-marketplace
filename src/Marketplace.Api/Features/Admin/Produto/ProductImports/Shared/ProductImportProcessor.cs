using System.Diagnostics;
using System.Text.Json;
using Marketplace.Api.Domain;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Produto.ProductImports.Shared;

public sealed class ProductImportProcessor(
    MarketplaceDbContext db,
    IWebHostEnvironment environment,
    ProductImportImageDownloader imageDownloader,
    ILogger<ProductImportProcessor> logger)
{
    public async Task ProcessAsync(int jobId, CancellationToken cancellationToken)
    {
        var job = await db.ProductImportJobs
            .Include(item => item.Items)
            .FirstOrDefaultAsync(item => item.Id == jobId, cancellationToken);
        if (job is null)
        {
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        await MarkJobAsProcessingAsync(job, cancellationToken);

        var downloadedFiles = new List<string>();
        try
        {
            var absolutePath = ProductImportFiles.ToAbsolutePath(environment.WebRootPath, job.StoredFilePath);
            var rows = ProductImportWorkbook.ReadRows(await File.ReadAllBytesAsync(absolutePath, cancellationToken));
            var validation = await ValidateRowsAsync(rows, cancellationToken);

            if (validation.ErrorItems.Count > 0)
            {
                await CompleteFailureAsync(
                    job,
                    stopwatch,
                    rows,
                    validation.ErrorItems,
                    "Importacao falhou. Corrija os erros da planilha e envie novamente.",
                    cancellationToken);
                return;
            }

            foreach (var row in rows)
            {
                var images = await Task.Run(
                        () => imageDownloader.DownloadImagesAsync(row.Sku, row.ImageUrls, cancellationToken),
                        cancellationToken)
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
        catch (ProductImportException exception)
        {
            DeleteDownloadedFiles(downloadedFiles);
            logger.LogWarning(exception, "Product import job {JobId} failed.", job.Id);
            await CompleteFailureAsync(
                job,
                stopwatch,
                [],
                [
                    new ProductImportJobItem
                    {
                        RowNumber = exception.RowNumber,
                        Sku = exception.Sku,
                        Title = exception.Title,
                        Status = ProductImportJobItemStatus.Error,
                        ErrorMessage = exception.Message
                    }
                ],
                "Importacao falhou durante o processamento.",
                cancellationToken);
        }
        catch (Exception exception)
        {
            DeleteDownloadedFiles(downloadedFiles);
            logger.LogError(exception, "Product import job {JobId} failed.", job.Id);
            await CompleteFailureAsync(
                job,
                stopwatch,
                [],
                [
                    new ProductImportJobItem
                    {
                        Status = ProductImportJobItemStatus.Error,
                        ErrorMessage = "Erro inesperado durante a importacao."
                    }
                ],
                "Importacao falhou durante o processamento.",
                cancellationToken);
        }
    }

    private async Task MarkJobAsProcessingAsync(ProductImportJob job, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        job.Status = ProductImportJobStatus.Processing;
        job.StartedAt = DateTimeOffset.UtcNow;
        job.FinishedAt = null;
        job.DurationMs = null;
        job.SummaryMessage = "Importacao em processamento.";
        db.ProductImportJobItems.RemoveRange(job.Items);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task CompleteFailureAsync(
        ProductImportJob job,
        Stopwatch stopwatch,
        IReadOnlyList<ProductImportRow> rows,
        IReadOnlyList<ProductImportJobItem> items,
        string message,
        CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        CompleteFailed(job, stopwatch, rows, items, message);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task<ProductImportValidation> ValidateRowsAsync(
        IReadOnlyList<ProductImportRow> rows,
        CancellationToken cancellationToken)
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

        var sellerLogins = rows
            .Select(row => row.LoginVendedor)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
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
                var subCategory = category.SubCategories.FirstOrDefault(item =>
                    ProductImportWorkbook.NormalizeKey(item.Title) == ProductImportWorkbook.NormalizeKey(row.SubCategory));
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
                if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
                    (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
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

    private async Task<List<ProductImportJobItem>> UpsertProductsAsync(
        IReadOnlyList<ProductImportRow> rows,
        ProductImportValidation validation,
        CancellationToken cancellationToken)
    {
        var normalizedAttributeNames = rows
            .SelectMany(row => row.Attributes.Keys)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var existingAttributes = await db.Attributes.ToListAsync(cancellationToken);
        var attributesByName = existingAttributes.ToDictionary(
            item => ProductImportWorkbook.NormalizeKey(item.Name),
            item => item);

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
            product.Images = Marketplace.Api.Features.Website.Produto.Shared.ProductImageStorage
                .NormalizeFileNames(row.DownloadedImages)
                .Select(image => new ProductImage { FileName = image })
                .ToList();
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

    private static void CompleteSuccess(
        ProductImportJob job,
        Stopwatch stopwatch,
        IReadOnlyList<ProductImportRow> rows,
        IReadOnlyList<ProductImportJobItem> items)
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

    private void CompleteFailed(
        ProductImportJob job,
        Stopwatch stopwatch,
        IReadOnlyList<ProductImportRow> rows,
        IReadOnlyList<ProductImportJobItem> items,
        string message)
    {
        stopwatch.Stop();
        job.Status = ProductImportJobStatus.Failed;
        job.FinishedAt = DateTimeOffset.UtcNow;
        job.DurationMs = stopwatch.ElapsedMilliseconds;
        job.TotalRows = rows.Count;
        job.SkuCount = rows
            .Where(row => !string.IsNullOrWhiteSpace(row.Sku))
            .Select(row => row.Sku)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();
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

