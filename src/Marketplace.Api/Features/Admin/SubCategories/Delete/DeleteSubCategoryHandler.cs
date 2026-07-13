using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.SubCategories.Delete;

public sealed class DeleteSubCategoryHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(int id, CancellationToken cancellationToken)
    {
        var subCategory = await db.SubCategories.FindAsync([id], cancellationToken);
        if (subCategory is null)
        {
            return Results.NotFound();
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        await db.Products.Where(product => product.SubCategoryId == id)
            .ExecuteUpdateAsync(setters => setters.SetProperty(product => product.SubCategoryId, (int?)null), cancellationToken);
        db.SubCategories.Remove(subCategory);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Results.NoContent();
    }
}
