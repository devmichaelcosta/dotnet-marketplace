using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Categories.Delete;

public sealed class DeleteCategoryHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(int id, CancellationToken cancellationToken)
    {
        var category = await db.Categories
            .Include(item => item.SubCategories)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (category is null)
        {
            return Results.NotFound();
        }

        var subCategoryIds = category.SubCategories.Select(item => item.Id).ToArray();

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        await db.Products
            .Where(product => product.SubCategoryId != null && subCategoryIds.Contains(product.SubCategoryId.Value))
            .ExecuteUpdateAsync(setters => setters.SetProperty(product => product.SubCategoryId, (int?)null), cancellationToken);
        db.Categories.Remove(category);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Results.NoContent();
    }
}
