namespace Marketplace.Api.Features.Website.Cart.GetCart;

public static class GetCartEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            HttpContext http,
            GetCartHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(http, cancellationToken));
    }
}
