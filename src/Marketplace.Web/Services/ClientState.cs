using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Marketplace.Web.Services;

public sealed class ClientState
{
    public event Action? Changed;

    public string? Token { get; private set; }
    public string? UserName { get; private set; }
    public string[] Roles { get; private set; } = [];
    public string CartId { get; private set; } = Guid.NewGuid().ToString("N");

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);
    public bool IsAdmin => Roles.Contains("admin");
    public bool IsSeller => Roles.Contains("vendedor");
    public bool IsCustomer => Roles.Contains("comum");

    public ClientState()
    {
    }

    public ClientState(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            Restore(user);
        }
    }

    public void Restore(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            SignOut();
            return;
        }

        var token = principal.FindFirst(MarketplaceAuthDefaults.AccessTokenClaim)?.Value;
        var userName = principal.Identity.Name ?? principal.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(userName))
        {
            SignOut();
            return;
        }

        Token = token;
        UserName = userName;
        Roles = principal.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToArray();
        Changed?.Invoke();
    }

    public void UpdateCartId(string cartId)
    {
        if (!string.IsNullOrWhiteSpace(cartId) && CartId != cartId)
        {
            CartId = cartId;
            Changed?.Invoke();
        }
    }

    public void SignOut()
    {
        Token = null;
        UserName = null;
        Roles = [];
        Changed?.Invoke();
    }

    public void NotifyChanged() => Changed?.Invoke();
}
