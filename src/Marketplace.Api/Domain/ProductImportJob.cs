namespace Marketplace.Api.Domain;

public sealed class ProductImportJob
{
    public int Id { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string StoredFilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public Guid ImportedByUserId { get; set; }
    public string ImportedByName { get; set; } = string.Empty;
    public ProductImportJobStatus Status { get; set; } = ProductImportJobStatus.Pending;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }
    public long? DurationMs { get; set; }
    public int TotalRows { get; set; }
    public int SkuCount { get; set; }
    public int CreatedCount { get; set; }
    public int UpdatedCount { get; set; }
    public int ErrorCount { get; set; }
    public string SummaryMessage { get; set; } = string.Empty;
    public List<ProductImportJobItem> Items { get; set; } = [];
}

public enum ProductImportJobStatus
{
    Pending = 0,
    Processing = 1,
    Success = 2,
    Failed = 3
}
