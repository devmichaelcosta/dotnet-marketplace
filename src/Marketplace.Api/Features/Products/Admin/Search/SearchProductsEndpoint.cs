namespace Marketplace.Api.Features.Products.Admin.Search;

public static class SearchProductsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            [AsParameters] SearchProductsQuery query,
            HttpContext http,
            SearchProductsHandler handler,
            CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(query, http, cancellationToken)));
    }
}