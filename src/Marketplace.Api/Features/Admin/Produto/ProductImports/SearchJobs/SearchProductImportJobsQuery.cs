namespace Marketplace.Api.Features.Admin.Produto.ProductImports.SearchJobs;

public sealed class SearchProductImportJobsQuery
{
    public string? Search { get; init; }
    public string? Status { get; init; }
    public string? Sort { get; init; }
    public string? Direction { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}

