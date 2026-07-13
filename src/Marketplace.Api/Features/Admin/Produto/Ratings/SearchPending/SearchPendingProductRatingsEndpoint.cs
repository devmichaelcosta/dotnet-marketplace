namespace Marketplace.Api.Features.Admin.Produto.Ratings.SearchPending;

public static class SearchPendingProductRatingsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/pending", async (
            [AsParameters] SearchPendingProductRatingsQuery query,
            SearchPendingProductRatingsHandler handler,
            CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(query, cancellationToken)));
    }
}

