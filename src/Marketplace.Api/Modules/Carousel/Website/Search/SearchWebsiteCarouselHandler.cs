using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Modules.Carousel.Website.Search;

public sealed class SearchWebsiteCarouselHandler(MarketplaceDbContext db)
{
    public async Task<List<WebsiteCarouselItemResponse>> HandleAsync(CancellationToken cancellationToken)
    {
        return await db.CarouselImages
            .AsNoTracking()
            .OrderBy(item => item.SortOrder)
            .Select(item => new WebsiteCarouselItemResponse(item.Id, item.FileName, item.SortOrder))
            .ToListAsync(cancellationToken);
    }
}
