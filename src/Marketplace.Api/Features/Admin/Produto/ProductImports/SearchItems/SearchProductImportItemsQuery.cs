namespace Marketplace.Api.Features.Admin.Produto.ProductImports.SearchItems;

public sealed class SearchProductImportItemsQuery
{
    public int Id { get; init; }
    public string? Search { get; init; }
    public string? Status { get; init; }
    public string? Sort { get; init; }
    public string? Direction { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}

