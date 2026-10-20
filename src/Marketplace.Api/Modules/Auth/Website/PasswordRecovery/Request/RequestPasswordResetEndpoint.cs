using Microsoft.AspNetCore.RateLimiting;

namespace Marketplace.Api.Modules.Auth.Website.PasswordRecovery.Request;

public static class RequestPasswordResetEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/password-reset/request", HandleAsync)
            .RequireRateLimiting("password-reset-request");
    }

    private static async Task<IResult> HandleAsync(
        RequestPasswordResetRequest request,
        RequestPasswordResetHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(request.Login, cancellationToken);
        return Results.Ok(response);
    }
}