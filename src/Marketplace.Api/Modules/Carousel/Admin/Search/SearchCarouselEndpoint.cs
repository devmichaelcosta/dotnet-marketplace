namespace Marketplace.Api.Modules.Carousel.Admin.Search;

public static class SearchCarouselEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            [AsParameters] SearchCarouselQuery query,
            SearchCarouselHandler handler,
            CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(query, cancellationToken)));
    }
}
