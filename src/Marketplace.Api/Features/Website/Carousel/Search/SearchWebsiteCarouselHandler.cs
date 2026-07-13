using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Website.Carousel.Search;

public sealed class SearchWebsiteCarouselHandler(MarketplaceDbContext db)
{
    public async Task<List<WebsiteCarouselItemResponse>> HandleAsync(CancellationToken cancellationToken)
    {
        return await db.CarouselImages
            .OrderBy(item => item.SortOrder)
            .Select(item => new WebsiteCarouselItemResponse(item.Id, item.FileName, item.SortOrder))
            .ToListAsync(cancellationToken);
    }
}
