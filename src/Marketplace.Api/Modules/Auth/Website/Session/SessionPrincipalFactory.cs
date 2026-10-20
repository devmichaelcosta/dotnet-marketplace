using System.Security.Claims;
using Marketplace.Api.Modules.Auth.Website.Login;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Marketplace.Api.Modules.Auth.Website.Session;

public static class SessionPrincipalFactory
{
    public const string AccessTokenClaim = "marketplace:access_token";

    public static ClaimsPrincipal Create(LoginResponse login)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, login.User.Id.ToString()),
            new(ClaimTypes.Name, login.User.UserName),
            new(AccessTokenClaim, login.Token)
        };
        claims.AddRange(login.User.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
    }
}
