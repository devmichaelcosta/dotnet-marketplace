namespace Marketplace.Api.Modules.Cart.Website.Checkout;

public static class CheckoutEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/checkout", async (
            CheckoutRequest request,
            HttpContext http,
            CheckoutHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(request, http, cancellationToken))
            .RequireAuthorization();
    }
}
