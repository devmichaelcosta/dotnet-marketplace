namespace Marketplace.Api.Modules.Produto.Website.GetLiked;

public static class GetLikedProductsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/liked", async (
            HttpContext http,
            GetLikedProductsHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(http, cancellationToken))
            .RequireAuthorization();
    }
}

