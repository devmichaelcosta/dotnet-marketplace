using Marketplace.Api.Modules.Catalog.Website.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Modules.Cart.Website.Shared;

public static class CartExtensions
{
    public static async Task<Marketplace.Api.Domain.Cart> GetOrCreateCartAsync(
        this MarketplaceDbContext db,
        HttpRequest request,
        Guid? userId,
        CancellationToken cancellationToken)
    {
        var anonymousKey = request.Headers.TryGetValue("X-Cart-Id", out var cartId)
            ? cartId.ToString()
            : string.Empty;

        if (string.IsNullOrWhiteSpace(anonymousKey))
        {
            anonymousKey = Guid.NewGuid().ToString("N");
        }

        var anonymousCart = await db.Carts
            .Include(item => item.Items)
            .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(item => item.AnonymousKey == anonymousKey, cancellationToken);

        var userCart = userId is null
            ? null
            : await db.Carts
                .Include(item => item.Items)
                .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken);

        if (anonymousCart is null && userCart is null)
        {
            var cart = new Marketplace.Api.Domain.Cart { AnonymousKey = anonymousKey, UserId = userId };
            db.Carts.Add(cart);
            await db.SaveChangesAsync(cancellationToken);
            return cart;
        }

        if (userCart is null)
        {
            if (anonymousCart is not null && userId is not null)
            {
                anonymousCart.UserId = userId;
                anonymousCart.UpdatedAt = DateTimeOffset.UtcNow;
                await db.SaveChangesAsync(cancellationToken);
            }

            return anonymousCart!;
        }

        if (anonymousCart is null || ReferenceEquals(anonymousCart, userCart))
        {
            if (userCart.UserId is null && userId is not null)
            {
                userCart.UserId = userId;
                userCart.UpdatedAt = DateTimeOffset.UtcNow;
                await db.SaveChangesAsync(cancellationToken);
            }

            return userCart;
        }

        var stockByProductId = userCart.Items
            .Concat(anonymousCart.Items)
            .GroupBy(item => item.ProductId)
            .ToDictionary(group => group.Key, group => group.First().Product?.Stock ?? 0);

        CartPolicy.MergeItems(userCart, anonymousCart, stockByProductId, anonymousKey);
        userCart.UserId = userId;
        userCart.UpdatedAt = DateTimeOffset.UtcNow;
        db.Carts.Remove(anonymousCart);
        await db.SaveChangesAsync(cancellationToken);

        return userCart;
    }

    public static object ToResponse(this Marketplace.Api.Domain.Cart cart)
    {
        var items = cart.Items.Select(item => new
        {
            item.ProductId,
            item.Product!.Title,
            item.Quantity,
            item.UnitPrice,
            SubTotal = item.UnitPrice * item.Quantity,
            Image = ProductImagePath.Normalize(item.Product.Images.FirstOrDefault()?.FileName)
        }).ToArray();

        return new
        {
            CartId = cart.AnonymousKey,
            Items = items,
            SubTotal = items.Sum(item => item.SubTotal),
            Shipping = 0m,
            Total = items.Sum(item => item.SubTotal)
        };
    }
}
