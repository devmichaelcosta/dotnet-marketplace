using Marketplace.Api.Domain;
using Marketplace.Api.Shared.Admin;
using Marketplace.Api.Modules.SubCategories.Admin.Search;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Modules.SubCategories.Admin.Create;

public sealed class CreateSubCategoryHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(CreateSubCategoryRequest request, CancellationToken cancellationToken)
    {
        var validationErrors = AdminValidationPolicy.ValidateSubCategory(request);
        if (validationErrors.Count > 0)
        {
            return Results.ValidationProblem(validationErrors);
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var subCategory = new SubCategory { CategoryId = request.CategoryId, Title = request.Title.Trim() };
        db.SubCategories.Add(subCategory);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Results.Created($"/api/admin/subcategories/{subCategory.Id}", SubCategoryResponse.From(subCategory));
    }
}
