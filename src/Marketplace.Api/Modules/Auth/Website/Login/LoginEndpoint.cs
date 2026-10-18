using Marketplace.Api.Modules.Auth.Website.Shared;

namespace Marketplace.Api.Modules.Auth.Website.Login;

public static class LoginEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/login", async (
            LoginRequest request,
            LoginHandler handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(request, cancellationToken);
            return result is null ? Results.Unauthorized() : Results.Ok(result);
        });
    }
}
