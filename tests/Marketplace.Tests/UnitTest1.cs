using Marketplace.Api.Domain;
using Marketplace.Web.Services;

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
    public void ClientState_can_sign_in_and_out()
    {
        var state = new ClientState();

        state.SignIn("token", "michael", ["admin"]);

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
}
