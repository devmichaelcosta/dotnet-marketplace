namespace Marketplace.Api.Features.Website.Cart.DeleteItem;

public static class DeleteCartItemEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/items/{productId:int}", async (
            int productId,
            HttpContext http,
            DeleteCartItemHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(productId, http, cancellationToken));
    }
}
