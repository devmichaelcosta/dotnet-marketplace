using System.Security.Claims;
using Marketplace.Web.Services;
using Microsoft.AspNetCore.Http;

namespace Marketplace.Tests;

public sealed class MarketplaceSmokeTests
{
    [Fact]
    public void CartItem_keeps_order_unit_price_snapshot()
    {
        var item = new Marketplace.Api.Domain.CartItem
        {
            ProductId = 10,
            Quantity = 3,
            UnitPrice = 12.5m
        };

        Assert.Equal(37.5m, item.Quantity * item.UnitPrice);
    }

    [Fact]
    public void ClientState_starts_with_anonymous_cart_id()
    {
        var state = new ClientState();

        Assert.False(state.IsAuthenticated);
        Assert.False(string.IsNullOrWhiteSpace(state.CartId));
    }

    [Fact]
    public void ClientState_can_restore_claims_principal_and_sign_out()
    {
        var state = new ClientState();

        state.Restore(CreatePrincipal("token", "michael", ["admin"]));

        Assert.True(state.IsAuthenticated);
        Assert.True(state.IsAdmin);
        Assert.Equal("michael", state.UserName);

        state.SignOut();

        Assert.False(state.IsAuthenticated);
        Assert.Null(state.UserName);
        Assert.Empty(state.Roles);
    }

    [Fact]
    public void ClientState_updates_cart_id_when_api_returns_persisted_cart()
    {
        var state = new ClientState();

        state.UpdateCartId("persisted-cart");

        Assert.Equal("persisted-cart", state.CartId);
    }

    [Fact]
    public void ClientState_restores_authenticated_principal()
    {
        var state = new ClientState();
        state.UpdateCartId("cart-cookie");

        state.Restore(CreatePrincipal("token", "michael", ["admin"]));

        Assert.True(state.IsAuthenticated);
        Assert.True(state.IsAdmin);
        Assert.Equal("michael", state.UserName);
        Assert.Equal("cart-cookie", state.CartId);
    }

    [Fact]
    public void ClientState_initializes_from_authenticated_http_context_user()
    {
        var httpContext = new DefaultHttpContext
        {
            User = CreatePrincipal("server-cookie-token", "michael", ["admin"])
        };
        var accessor = new HttpContextAccessor { HttpContext = httpContext };

        var state = new ClientState(accessor);

        Assert.True(state.IsAuthenticated);
        Assert.True(state.IsAdmin);
        Assert.Equal("michael", state.UserName);
        Assert.Equal("server-cookie-token", state.Token);
    }

    private static ClaimsPrincipal CreatePrincipal(string token, string userName, string[] roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userName),
            new(MarketplaceAuthDefaults.AccessTokenClaim, token)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies"));
    }
}
