using Marketplace.Api.Features.Admin.Shared;
using Marketplace.Api.Features.Admin.SubCategories.Search;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.SubCategories.Update;

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
