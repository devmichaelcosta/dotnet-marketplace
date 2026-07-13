using Marketplace.Api.Features.Admin.Carousel.Search;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.Carousel.Update;

public sealed class UpdateCarouselHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(int id, UpdateCarouselRequest request, CancellationToken cancellationToken)
    {
        var image = await db.CarouselImages.FindAsync([id], cancellationToken);
        if (image is null)
        {
            return Results.NotFound();
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        image.FileName = request.FileName.Trim();
        image.SortOrder = request.SortOrder;
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Results.Ok(new CarouselResponse(image.Id, image.FileName, image.SortOrder));
    }
}
