using Marketplace.Api.Features.Website.Auth.Shared;

namespace Marketplace.Api.Features.Website.Auth.Login;

public static class LoginEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/login", async (
            LoginRequest request,
            LoginHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(request, cancellationToken));
    }
}
