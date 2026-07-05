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
}
