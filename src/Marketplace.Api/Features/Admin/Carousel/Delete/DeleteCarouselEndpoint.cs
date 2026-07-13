namespace Marketplace.Api.Features.Admin.Carousel.Delete;

public static class DeleteCarouselEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:int}", async (
            int id,
            DeleteCarouselHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, cancellationToken));
    }
}
