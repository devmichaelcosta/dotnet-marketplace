using Marketplace.Api.Modules.Catalog.Website.Shared;
using Marketplace.Api.Modules.Carousel.Website.Search;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Modules.Catalog.Website.GetHome;

public sealed class GetHomeHandler(
    MarketplaceDbContext db,
    SearchWebsiteCarouselHandler carouselHandler)
{
    public async Task<object> HandleAsync(CancellationToken cancellationToken)
    {
        var carousel = await carouselHandler.HandleAsync(cancellationToken);
        var categories = await db.Categories
            .AsNoTracking()
            .OrderBy(item => item.Title)
            .ToListAsync(cancellationToken);
        var offers = await db.Products
            .AsNoTracking()
            .Where(product => product.Offer && product.Stock > 0)
            .OrderBy(product => product.Title)
            .ThenBy(product => product.Id)
            .Take(12)
            .Select(ProductSummary.Projection)
            .ToListAsync(cancellationToken);

        return new { Carousel = carousel, Categories = categories, Offers = offers };
    }
}
