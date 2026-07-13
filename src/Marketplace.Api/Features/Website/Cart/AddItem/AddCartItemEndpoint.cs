namespace Marketplace.Api.Features.Website.Cart.AddItem;

public static class AddCartItemEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/items", async (
            CartItemRequest request,
            HttpContext http,
            AddCartItemHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(request, http, cancellationToken));
    }
}
