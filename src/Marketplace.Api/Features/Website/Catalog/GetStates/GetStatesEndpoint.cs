namespace Marketplace.Api.Features.Website.Catalog.GetStates;

public static class GetStatesEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/states", async (
            GetStatesHandler handler,
            CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(cancellationToken)));
    }
}
