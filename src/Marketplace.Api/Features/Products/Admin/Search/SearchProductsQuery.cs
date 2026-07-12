namespace Marketplace.Api.Features.Products.Admin.Search;

public sealed class SearchProductsQuery
{
    public string? Search { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Sort { get; set; }
    public string? Direction { get; set; }
}