using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Carousel.Search;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.Carousel.Create;

public sealed class CreateCarouselHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(CreateCarouselRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FileName))
        {
            return Results.ValidationProblem(new Dictionary<string, string[]> { ["fileName"] = ["Imagem obrigatoria."] });
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var image = new CarouselImage
        {
            FileName = request.FileName.Trim(),
            SortOrder = request.SortOrder
        };
        db.CarouselImages.Add(image);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Results.Created($"/api/admin/carousel/{image.Id}", new CarouselResponse(image.Id, image.FileName, image.SortOrder));
    }
}
