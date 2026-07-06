using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Marketplace.Web.Services;

public sealed class MarketplaceAuthService(HttpClient api)
{
    public async Task<ClaimsPrincipal?> AuthenticateAsync(WebLoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        using var response = await api.PostAsJsonAsync("api/auth/login", request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var login = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken);
        if (login is null || string.IsNullOrWhiteSpace(login.Token))
        {
            return null;
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, login.User.Id.ToString()),
            new(ClaimTypes.Name, login.User.UserName),
            new(MarketplaceAuthDefaults.AccessTokenClaim, login.Token)
        };
        claims.AddRange(login.User.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
    }
}
