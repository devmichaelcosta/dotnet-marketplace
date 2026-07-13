using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Shared;

public static class SellerDeletionPolicy
{
    public static async Task<IResult?> ValidateAsync(MarketplaceDbContext db, Guid userId, CancellationToken cancellationToken = default)
    {
        var hasProducts = await db.Products.AnyAsync(product => product.UserId == userId, cancellationToken);
        if (hasProducts)
        {
            return Results.Problem(
                "Nao e possivel excluir o vendedor enquanto existirem produtos vinculados.",
                statusCode: StatusCodes.Status409Conflict);
        }

        return null;
    }
}
