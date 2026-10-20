using Marketplace.Api.Modules.Categories.Admin.Search;
using Marketplace.Api.Shared.Admin;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Modules.Categories.Admin.Update;

public sealed class UpdateCategoryHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var validationErrors = AdminValidationPolicy.ValidateCategory(request);
        if (validationErrors.Count > 0)
        {
            return Results.ValidationProblem(validationErrors);
        }

        var category = await db.Categories.FindAsync([id], cancellationToken);
        if (category is null)
        {
            return Results.NotFound();
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        category.Title = request.Title.Trim();
        category.Image = request.Image;
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Results.Ok(CategoryResponse.From(category));
    }
}
