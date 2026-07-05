namespace Marketplace.Api.Domain;

public sealed class ProductImportJobItem
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public ProductImportJob? Job { get; set; }
    public int RowNumber { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public ProductImportJobItemStatus Status { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public int? ProductId { get; set; }
    public string DownloadedImages { get; set; } = string.Empty;
    public string ImportedAttributes { get; set; } = string.Empty;
}

public enum ProductImportJobItemStatus
{
    Created = 0,
    Updated = 1,
    Error = 2,
    Ignored = 3
}
