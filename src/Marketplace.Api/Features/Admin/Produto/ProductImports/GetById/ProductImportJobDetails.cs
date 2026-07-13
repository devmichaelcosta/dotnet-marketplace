using Marketplace.Api.Domain;

namespace Marketplace.Api.Features.Admin.Produto.ProductImports.GetById;

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

