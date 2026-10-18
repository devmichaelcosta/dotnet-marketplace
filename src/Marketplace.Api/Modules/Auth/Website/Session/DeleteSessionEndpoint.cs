using Marketplace.Api.Modules.Auth.Website.Login;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Marketplace.Api.Modules.Auth.Website.Session;

public static class DeleteSessionEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/logout", HandleAsync);
    }

    private static async Task<IResult> HandleAsync(IAntiforgery antiforgery, HttpContext http)
    {
        try
        {
            await antiforgery.ValidateRequestAsync(http);
        }
        catch (AntiforgeryValidationException)
        {
            return Results.BadRequest(new SessionLoginResponse(false, "Sessao expirada. Atualize a pagina e tente novamente."));
        }

        await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Results.Ok(new SessionLoginResponse(true, null));
    }
}
