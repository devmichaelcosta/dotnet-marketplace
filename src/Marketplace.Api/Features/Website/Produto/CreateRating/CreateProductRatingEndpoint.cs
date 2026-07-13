namespace Marketplace.Api.Features.Website.Produto.CreateRating;

public static class CreateProductRatingEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{id:int}/ratings", async (
            int id,
            CreateProductRatingRequest request,
            HttpContext http,
            CreateProductRatingHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, request, http, cancellationToken))
            .RequireAuthorization();
    }
}

