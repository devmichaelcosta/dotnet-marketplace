namespace Marketplace.Api.Features.Admin.Produto.Ratings.SearchPending;

public sealed class SearchPendingProductRatingsQuery
{
    public string? Search { get; init; }
    public string? Sort { get; init; }
    public string? Direction { get; init; }
}

