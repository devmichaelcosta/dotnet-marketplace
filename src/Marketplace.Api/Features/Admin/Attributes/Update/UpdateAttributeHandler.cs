using Marketplace.Api.Features.Admin.Attributes.Search;
using Marketplace.Api.Features.Admin.Shared;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.Attributes.Update;

public sealed class UpdateAttributeHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(int id, UpdateAttributeRequest request, CancellationToken cancellationToken)
    {
        var validationErrors = AdminValidationPolicy.ValidateAttribute(request);
        if (validationErrors.Count > 0)
        {
            return Results.ValidationProblem(validationErrors);
        }

        var attribute = await db.Attributes.FindAsync([id], cancellationToken);
        if (attribute is null)
        {
            return Results.NotFound();
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        attribute.Name = request.Name.Trim();
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Results.Ok(new AttributeResponse(attribute.Id, attribute.Name));
    }
}
