using Marketplace.Api.Modules.Auth.Website.Login;
using Marketplace.Api.Modules.Auth.Website.Shared;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Marketplace.Api.Modules.Auth.Website.Session;

public static class CreateSessionEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/login", HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        LoginRequest request,
        LoginHandler loginHandler,
        IAntiforgery antiforgery,
        HttpContext http,
        CancellationToken cancellationToken)
    {
        try
        {
            await antiforgery.ValidateRequestAsync(http);
        }
        catch (AntiforgeryValidationException)
        {
            return Results.BadRequest(new SessionLoginResponse(false, "Sessao expirada. Atualize a pagina e tente novamente."));
        }

        if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Results.BadRequest(new SessionLoginResponse(false, "Informe login e senha."));
        }

        var login = await loginHandler.HandleAsync(request, cancellationToken);
        if (login is null)
        {
            return Results.Unauthorized();
        }

        await http.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            SessionPrincipalFactory.Create(login),
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                AllowRefresh = false
            });

        return Results.Ok(new SessionLoginResponse(true, null));
    }
}
