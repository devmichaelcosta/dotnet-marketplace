using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Attributes.Search;
using Marketplace.Api.Features.Admin.Shared;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.Attributes.Create;

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
