using Marketplace.Api.Features.Website.Auth.Shared;

namespace Marketplace.Api.Features.Website.Auth.RegisterSeller;

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
