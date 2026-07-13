namespace Marketplace.Api.Features.Admin.Carousel.Update;

public static class UpdateCarouselEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{id:int}", async (
            int id,
            UpdateCarouselRequest request,
            UpdateCarouselHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, request, cancellationToken));
    }
}
