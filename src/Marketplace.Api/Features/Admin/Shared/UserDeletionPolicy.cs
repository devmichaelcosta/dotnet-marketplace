using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Shared;

public static class UserDeletionPolicy
{
    public static async Task<IResult?> ValidateAsync(MarketplaceDbContext db, Guid userId, CancellationToken cancellationToken = default)
    {
        var hasSeller = await db.Sellers.AnyAsync(seller => seller.UserId == userId, cancellationToken);
        var hasProducts = await db.Products.AnyAsync(product => product.UserId == userId, cancellationToken);
        var hasOrders = await db.Orders.AnyAsync(order => order.UserId == userId, cancellationToken);
        var hasRatings = await db.ProductRatings.AnyAsync(rating => rating.UserId == userId, cancellationToken);
        var hasLikes = await db.ProductLikes.AnyAsync(like => like.UserId == userId, cancellationToken);
        var hasCarts = await db.Carts.AnyAsync(cart => cart.UserId == userId, cancellationToken);

        if (hasSeller || hasProducts || hasOrders || hasRatings || hasLikes || hasCarts)
        {
            return Results.Problem(
                "Nao e possivel excluir o usuario porque existem vinculos administrativos ou transacionais.",
                statusCode: StatusCodes.Status409Conflict);
        }

        return null;
    }
}
