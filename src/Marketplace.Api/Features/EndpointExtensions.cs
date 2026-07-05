using System.Security.Claims;
using Marketplace.Api.Domain;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features;

public static class EndpointExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(id, out var value) ? value : null;
    }

    public static async Task<Domain.Cart> GetOrCreateCartAsync(
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

        var cart = await db.Carts
            .Include(item => item.Items)
            .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(item => item.AnonymousKey == anonymousKey || (userId != null && item.UserId == userId), cancellationToken);

        if (cart is null)
        {
            cart = new Domain.Cart { AnonymousKey = anonymousKey, UserId = userId };
            db.Carts.Add(cart);
            await db.SaveChangesAsync(cancellationToken);
            return cart;
        }

        if (userId is not null && cart.UserId is null)
        {
            cart.UserId = userId;
            cart.UpdatedAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(cancellationToken);
        }

        return cart;
    }

    public static object ToResponse(this Domain.Cart cart)
    {
        var items = cart.Items.Select(item => new
        {
            item.ProductId,
            item.Product!.Title,
            item.Quantity,
            item.UnitPrice,
            SubTotal = item.UnitPrice * item.Quantity,
            Image = item.Product.Images.FirstOrDefault()?.FileName
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

    public static Results<Ok<T>, NotFound> ToOkOrNotFound<T>(this T? result)
    {
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }
}
