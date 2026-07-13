using Marketplace.Api.Features.Website.Auth.Shared;

namespace Marketplace.Api.Features.Website.Auth.Register;

public static class RegisterEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/register", async (
            RegisterRequest request,
            RegisterHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(request, cancellationToken));
    }
}
