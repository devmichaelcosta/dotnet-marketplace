using Marketplace.Api.Domain;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Produto.SaveSimilarProducts;

public sealed class SaveSimilarProductsHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(int id, SaveSimilarProductsRequest request, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        await db.SimilarProducts.Where(item => item.ParentProductId == id).ExecuteDeleteAsync(cancellationToken);
        db.SimilarProducts.AddRange(request.ProductIds
            .Where(childId => childId != id)
            .Distinct()
            .Select(childId => new SimilarProduct
            {
                ParentProductId = id,
                ChildProductId = childId
            }));

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Results.NoContent();
    }
}

