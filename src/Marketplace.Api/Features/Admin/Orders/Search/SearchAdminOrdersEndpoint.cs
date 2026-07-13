namespace Marketplace.Api.Features.Admin.Orders.Search;

public static class SearchAdminOrdersEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            [AsParameters] SearchAdminOrdersQuery query,
            SearchAdminOrdersHandler handler,
            CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(query, cancellationToken)));
    }
}
