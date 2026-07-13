using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Categories.Search;
using Marketplace.Api.Features.Admin.Shared;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.Categories.Create;

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
