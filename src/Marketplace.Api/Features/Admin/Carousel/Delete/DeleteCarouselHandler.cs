using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.Carousel.Delete;

public sealed class DeleteCarouselHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(int id, CancellationToken cancellationToken)
    {
        var image = await db.CarouselImages.FindAsync([id], cancellationToken);
        if (image is null)
        {
            return Results.NotFound();
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        db.CarouselImages.Remove(image);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Results.NoContent();
    }
}
