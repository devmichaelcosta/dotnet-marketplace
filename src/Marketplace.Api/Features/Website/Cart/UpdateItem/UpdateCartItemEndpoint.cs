namespace Marketplace.Api.Features.Website.Cart.UpdateItem;

public static class UpdateCartItemEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/items/{productId:int}", async (
            int productId,
            UpdateCartItemRequest request,
            HttpContext http,
            UpdateCartItemHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(productId, request, http, cancellationToken));
    }
}
