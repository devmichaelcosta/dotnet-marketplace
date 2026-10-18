using Microsoft.AspNetCore.Antiforgery;

namespace Marketplace.Api.Modules.Auth.Website.Session;

public static class GetAntiforgeryTokenEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/antiforgery", (IAntiforgery antiforgery, HttpContext http) =>
        {
            var tokens = antiforgery.GetAndStoreTokens(http);
            return Results.Ok(new AntiforgeryTokenResponse(tokens.RequestToken ?? string.Empty));
        });
    }
}
