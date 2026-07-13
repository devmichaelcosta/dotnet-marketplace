namespace Marketplace.Api.Features.Admin.Categories.Search;

public static class SearchCategoriesEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            [AsParameters] SearchCategoriesQuery query,
            SearchCategoriesHandler handler,
            CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(query, cancellationToken)));
    }
}
