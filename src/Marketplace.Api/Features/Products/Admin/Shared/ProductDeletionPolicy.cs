using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Products.Admin.Shared;

public sealed class ProductDeletionPolicy
{
    public async Task<IResult?> ValidateAsync(MarketplaceDbContext db, int productId, CancellationToken cancellationToken = default)
    {
        var hasOrderItems = await db.OrderItems.AnyAsync(item => item.ProductId == productId, cancellationToken);
        if (hasOrderItems)
        {
            return Results.Problem(
                "Nao e possivel excluir o produto porque ele ja possui itens em pedidos.",
                statusCode: StatusCodes.Status409Conflict);
        }

        return null;
    }
}
