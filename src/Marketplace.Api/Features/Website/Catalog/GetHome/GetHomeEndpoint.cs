namespace Marketplace.Api.Features.Website.Catalog.GetHome;

public static class GetHomeEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/home", async (
            GetHomeHandler handler,
            CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(cancellationToken)));
    }
}
