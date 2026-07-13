namespace Marketplace.Api.Features.Admin.Sellers.Search;

public sealed class SearchSellersQuery
{
    public string? Search { get; init; }
    public string? Sort { get; init; }
    public string? Direction { get; init; }
}
