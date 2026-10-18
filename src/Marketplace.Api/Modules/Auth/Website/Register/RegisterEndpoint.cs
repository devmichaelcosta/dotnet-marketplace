using Marketplace.Api.Modules.Auth.Website.Shared;

namespace Marketplace.Api.Modules.Auth.Website.Register;

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
