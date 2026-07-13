namespace Marketplace.Api.Features.Admin.SubCategories.Search;

public static class SearchSubCategoriesEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            [AsParameters] SearchSubCategoriesQuery query,
            SearchSubCategoriesHandler handler,
            CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(query, cancellationToken)));
    }
}
