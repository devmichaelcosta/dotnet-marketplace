namespace Marketplace.Api.Features.Admin.Sellers.Search;

public static class SearchSellersEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            [AsParameters] SearchSellersQuery query,
            SearchSellersHandler handler,
            CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(query, cancellationToken)));
    }
}
