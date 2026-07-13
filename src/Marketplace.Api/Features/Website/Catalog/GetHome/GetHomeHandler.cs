using Marketplace.Api.Features.Website.Carousel.Search;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Website.Catalog.GetHome;

public sealed class GetHomeHandler(
    MarketplaceDbContext db,
    SearchWebsiteCarouselHandler carouselHandler)
{
    public async Task<object> HandleAsync(CancellationToken cancellationToken)
    {
        var carousel = await carouselHandler.HandleAsync(cancellationToken);
        var categories = await db.Categories.OrderBy(item => item.Title).ToListAsync(cancellationToken);
        var offers = await db.Products
            .Include(product => product.Images)
            .Include(product => product.User)
            .Where(product => product.Offer && product.Stock > 0)
            .OrderBy(product => product.Title)
            .Take(12)
            .Select(product => ProductSummary.From(product))
            .ToListAsync(cancellationToken);

        return new { Carousel = carousel, Categories = categories, Offers = offers };
    }
}
