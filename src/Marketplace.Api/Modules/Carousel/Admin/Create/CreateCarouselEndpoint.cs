namespace Marketplace.Api.Modules.Carousel.Admin.Create;

public static class CreateCarouselEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            CreateCarouselRequest request,
            CreateCarouselHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(request, cancellationToken));
    }
}
