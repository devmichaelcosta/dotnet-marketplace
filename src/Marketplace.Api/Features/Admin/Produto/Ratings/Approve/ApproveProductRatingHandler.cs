using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.Produto.Ratings.Approve;

public sealed class ApproveProductRatingHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(int id, CancellationToken cancellationToken)
    {
        var rating = await db.ProductRatings.FindAsync([id], cancellationToken);
        if (rating is null)
        {
            return Results.NotFound();
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        rating.Approved = true;
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Results.NoContent();
    }
}

