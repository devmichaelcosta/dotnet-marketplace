using Microsoft.AspNetCore.RateLimiting;

namespace Marketplace.Api.Modules.Auth.Website.PasswordRecovery.Confirm;

public static class ResetPasswordEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/password-reset/confirm", HandleAsync)
            .RequireRateLimiting("password-reset-confirm");
    }

    private static async Task<IResult> HandleAsync(
        ResetPasswordRequest request,
        ResetPasswordHandler handler,
        CancellationToken cancellationToken)
    {
        var succeeded = await handler.HandleAsync(request, cancellationToken);
        return succeeded
            ? Results.Ok(new { message = "Senha redefinida. Voce ja pode entrar com a nova senha." })
            : Results.Problem(
                title: "Nao foi possivel redefinir a senha.",
                detail: "O link pode estar invalido ou expirado, ou a senha nao atende aos requisitos da conta.",
                statusCode: StatusCodes.Status400BadRequest);
    }
}