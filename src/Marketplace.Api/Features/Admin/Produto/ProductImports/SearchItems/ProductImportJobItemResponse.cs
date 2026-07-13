using Marketplace.Api.Domain;

namespace Marketplace.Api.Features.Admin.Produto.ProductImports.SearchItems;

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

