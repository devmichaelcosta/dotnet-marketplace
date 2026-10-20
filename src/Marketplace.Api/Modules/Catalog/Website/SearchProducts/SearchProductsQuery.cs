namespace Marketplace.Api.Modules.Catalog.Website.SearchProducts;

public sealed class SearchProductsQuery
{
    public string? Search { get; init; }
    public int? CategoryId { get; init; }
    public int? SubCategoryId { get; init; }
    public int Page { get; init; }
}
