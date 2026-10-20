namespace Marketplace.Api.Modules.Attributes.Admin.Search;

public static class SearchAttributesEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            [AsParameters] SearchAttributesQuery query,
            SearchAttributesHandler handler,
            CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(query, cancellationToken)));
    }
}
