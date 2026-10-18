using Marketplace.Api.Domain;
using Marketplace.Api.Modules.Attributes.Admin.Search;
using Marketplace.Api.Shared.Admin;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Modules.Attributes.Admin.Create;

public sealed class CreateAttributeHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(CreateAttributeRequest request, CancellationToken cancellationToken)
    {
        var validationErrors = AdminValidationPolicy.ValidateAttribute(request);
        if (validationErrors.Count > 0)
        {
            return Results.ValidationProblem(validationErrors);
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var attribute = new AttributeDefinition { Name = request.Name.Trim() };
        db.Attributes.Add(attribute);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Results.Created($"/api/admin/attributes/{attribute.Id}", new AttributeResponse(attribute.Id, attribute.Name));
    }
}
