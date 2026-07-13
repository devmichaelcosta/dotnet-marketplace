using Marketplace.Api.Features.Admin.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Carousel.Search;

public sealed class SearchCarouselHandler(MarketplaceDbContext db)
{
    public async Task<List<CarouselResponse>> HandleAsync(SearchCarouselQuery query, CancellationToken cancellationToken)
    {
        return await AdminListQueryPolicy.ApplyCarouselSort(
                db.CarouselImages.Where(item =>
                    string.IsNullOrWhiteSpace(query.Search) ||
                    item.FileName.Contains(query.Search) ||
                    item.SortOrder.ToString().Contains(query.Search)),
                query.Sort,
                query.Direction)
            .Select(item => new CarouselResponse(item.Id, item.FileName, item.SortOrder))
            .ToListAsync(cancellationToken);
    }
}
