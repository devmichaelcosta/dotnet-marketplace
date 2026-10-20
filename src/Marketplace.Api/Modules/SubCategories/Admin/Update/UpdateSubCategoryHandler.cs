using Marketplace.Api.Shared.Admin;
using Marketplace.Api.Modules.SubCategories.Admin.Search;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Modules.SubCategories.Admin.Update;

public sealed class UpdateSubCategoryHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(int id, UpdateSubCategoryRequest request, CancellationToken cancellationToken)
    {
        var validationErrors = AdminValidationPolicy.ValidateSubCategory(request);
        if (validationErrors.Count > 0)
        {
            return Results.ValidationProblem(validationErrors);
        }

        var subCategory = await db.SubCategories.FindAsync([id], cancellationToken);
        if (subCategory is null)
        {
            return Results.NotFound();
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        subCategory.CategoryId = request.CategoryId;
        subCategory.Title = request.Title.Trim();
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Results.Ok(SubCategoryResponse.From(subCategory));
    }
}
