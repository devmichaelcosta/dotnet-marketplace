namespace Marketplace.Api.Features.Admin.Users.Search;

public static class SearchUsersEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            [AsParameters] SearchUsersQuery query,
            SearchUsersHandler handler,
            CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(query, cancellationToken)));
    }
}
