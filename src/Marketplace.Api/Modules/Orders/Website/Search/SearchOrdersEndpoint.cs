namespace Marketplace.Api.Modules.Orders.Website.Search;

public static class SearchOrdersEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            HttpContext http,
            SearchOrdersHandler handler,
            CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(http, cancellationToken)));
    }
}
