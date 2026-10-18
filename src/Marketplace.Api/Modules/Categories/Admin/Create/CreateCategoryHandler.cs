using Marketplace.Api.Domain;
using Marketplace.Api.Modules.Categories.Admin.Search;
using Marketplace.Api.Shared.Admin;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Modules.Categories.Admin.Create;

public sealed class CreateCategoryHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var validationErrors = AdminValidationPolicy.ValidateCategory(request);
        if (validationErrors.Count > 0)
        {
            return Results.ValidationProblem(validationErrors);
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var category = new Category { Title = request.Title.Trim(), Image = request.Image };
        db.Categories.Add(category);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Results.Created($"/api/admin/categories/{category.Id}", CategoryResponse.From(category));
    }
}
