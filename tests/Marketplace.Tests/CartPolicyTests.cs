using Marketplace.Api.Domain;
using Marketplace.Api.Features.Cart;

namespace Marketplace.Tests;

public sealed class CartPolicyTests
{
    [Theory]
    [InlineData(0, 2, 2)]
    [InlineData(3, 4, 7)]
    public void Cart_policy_accumulates_existing_and_requested_quantities(int currentQuantity, int requestedQuantity, int expected)
    {
        Assert.Equal(expected, CartPolicy.CalculateNewQuantity(currentQuantity, requestedQuantity));
    }

    [Theory]
    [InlineData(10, 10, true)]
    [InlineData(10, 9, true)]
    [InlineData(10, 11, false)]
    public void Cart_policy_validates_available_stock(int stock, int requestedQuantity, bool expected)
    {
        Assert.Equal(expected, CartPolicy.HasAvailableStock(stock, requestedQuantity));
    }

    [Fact]
    public void Cart_policy_calculates_checkout_total_from_items()
    {
        var items = new[]
        {
            new CartItem { UnitPrice = 10.50m, Quantity = 2 },
            new CartItem { UnitPrice = 4m, Quantity = 3 }
        };

        Assert.Equal(33m, CartPolicy.CalculateTotal(items));
    }

    [Fact]
    public void Cart_policy_merges_items_and_keeps_anonymous_cart_key_when_cart_is_unified()
    {
        var targetCart = new Marketplace.Api.Domain.Cart
        {
            AnonymousKey = "user-cart",
            Items =
            [
                new CartItem { ProductId = 1, Quantity = 2, UnitPrice = 10m },
                new CartItem { ProductId = 2, Quantity = 1, UnitPrice = 20m }
            ]
        };
        var sourceCart = new Marketplace.Api.Domain.Cart
        {
            AnonymousKey = "anonymous-cart",
            Items =
            [
                new CartItem { ProductId = 1, Quantity = 4, UnitPrice = 12m },
                new CartItem { ProductId = 3, Quantity = 5, UnitPrice = 7m }
            ]
        };

        CartPolicy.MergeItems(
            targetCart,
            sourceCart,
            new Dictionary<int, int>
            {
                [1] = 5,
                [2] = 10,
                [3] = 2
            },
            "anonymous-cart");

        Assert.Equal("anonymous-cart", targetCart.AnonymousKey);
        Assert.Collection(
            targetCart.Items.OrderBy(item => item.ProductId),
            item =>
            {
                Assert.Equal(1, item.ProductId);
                Assert.Equal(5, item.Quantity);
                Assert.Equal(10m, item.UnitPrice);
            },
            item =>
            {
                Assert.Equal(2, item.ProductId);
                Assert.Equal(1, item.Quantity);
                Assert.Equal(20m, item.UnitPrice);
            },
            item =>
            {
                Assert.Equal(3, item.ProductId);
                Assert.Equal(2, item.Quantity);
                Assert.Equal(7m, item.UnitPrice);
            });
    }
}
