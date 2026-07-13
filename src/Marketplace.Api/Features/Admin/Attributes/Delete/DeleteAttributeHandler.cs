using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.Attributes.Delete;

public sealed class DeleteAttributeHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(int id, CancellationToken cancellationToken)
    {
        var attribute = await db.Attributes.FindAsync([id], cancellationToken);
        if (attribute is null)
        {
            return Results.NotFound();
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        db.Attributes.Remove(attribute);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Results.NoContent();
    }
}
