using Marketplace.Api.Modules.Attributes.Admin.Search;
using Marketplace.Api.Shared.Admin;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Modules.Attributes.Admin.Update;

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
