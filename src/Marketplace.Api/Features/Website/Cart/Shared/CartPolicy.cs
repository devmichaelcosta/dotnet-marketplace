using Marketplace.Api.Domain;

namespace Marketplace.Api.Features.Website.Cart.Shared;

public static class CartPolicy
{
    public static int CalculateNewQuantity(int currentQuantity, int requestedQuantity) => currentQuantity + requestedQuantity;

    public static bool HasAvailableStock(int stock, int requestedQuantity) => stock >= requestedQuantity;

    public static decimal CalculateTotal(IEnumerable<CartItem> items) => items.Sum(item => item.UnitPrice * item.Quantity);

    public static void MergeItems(Marketplace.Api.Domain.Cart targetCart, Marketplace.Api.Domain.Cart sourceCart, IReadOnlyDictionary<int, int> stockByProductId, string anonymousKey)
    {
        ArgumentNullException.ThrowIfNull(targetCart);
        ArgumentNullException.ThrowIfNull(sourceCart);
        ArgumentNullException.ThrowIfNull(stockByProductId);

        targetCart.AnonymousKey = anonymousKey;

        foreach (var sourceItem in sourceCart.Items)
        {
            var availableStock = stockByProductId.TryGetValue(sourceItem.ProductId, out var stock)
                ? stock
                : sourceItem.Quantity;

            var targetItem = targetCart.Items.FirstOrDefault(item => item.ProductId == sourceItem.ProductId);
            if (targetItem is null)
            {
                var quantity = Math.Min(availableStock, sourceItem.Quantity);
                if (quantity <= 0)
                {
                    continue;
                }

                targetCart.Items.Add(new CartItem
                {
                    ProductId = sourceItem.ProductId,
                    Quantity = quantity,
                    UnitPrice = sourceItem.UnitPrice,
                    Product = sourceItem.Product
                });
                continue;
            }

            targetItem.Quantity = Math.Min(availableStock, targetItem.Quantity + sourceItem.Quantity);
            if (targetItem.Quantity <= 0)
            {
                targetCart.Items.Remove(targetItem);
            }
        }
    }
}
