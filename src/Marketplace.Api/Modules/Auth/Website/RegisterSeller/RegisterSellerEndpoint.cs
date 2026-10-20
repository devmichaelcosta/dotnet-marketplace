using Marketplace.Api.Modules.Auth.Website.Shared;

namespace Marketplace.Api.Modules.Auth.Website.RegisterSeller;

public static class RegisterSellerEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/register-seller", async (
            RegisterRequest request,
            RegisterSellerHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(request, cancellationToken));
    }
}
