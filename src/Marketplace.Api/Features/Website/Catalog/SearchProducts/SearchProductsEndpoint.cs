namespace Marketplace.Api.Features.Website.Catalog.SearchProducts;

public static class SearchProductsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/products", async (
            [AsParameters] SearchProductsQuery query,
            SearchProductsHandler handler,
            CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(query, cancellationToken)));
    }
}
