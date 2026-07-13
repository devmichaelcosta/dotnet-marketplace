using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Produto.Ratings.SearchPending;

public sealed class SearchPendingProductRatingsHandler(MarketplaceDbContext db)
{
    public async Task<List<PendingProductRatingResponse>> HandleAsync(
        SearchPendingProductRatingsQuery query,
        CancellationToken cancellationToken)
    {
        var ratings = db.ProductRatings
            .Include(item => item.Product)
            .Include(item => item.User)
            .Where(item => !item.Approved);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            ratings = ratings.Where(item =>
                item.Product!.Title.Contains(query.Search) ||
                item.User!.Name.Contains(query.Search) ||
                item.Title.Contains(query.Search) ||
                item.Description.Contains(query.Search));
        }

        ratings = (query.Sort?.ToLowerInvariant(), query.Direction?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true) switch
        {
            ("product", true) => ratings.OrderByDescending(item => item.Product!.Title),
            ("product", false) => ratings.OrderBy(item => item.Product!.Title),
            ("user", true) => ratings.OrderByDescending(item => item.User!.Name),
            ("user", false) => ratings.OrderBy(item => item.User!.Name),
            ("title", true) => ratings.OrderByDescending(item => item.Title),
            ("title", false) => ratings.OrderBy(item => item.Title),
            ("rating", true) => ratings.OrderByDescending(item => item.Rating),
            ("rating", false) => ratings.OrderBy(item => item.Rating),
            ("created", false) => ratings.OrderBy(item => item.CreatedAt),
            _ => ratings.OrderByDescending(item => item.CreatedAt)
        };

        return await ratings
            .Select(item => new PendingProductRatingResponse(
                item.Id,
                item.ProductId,
                item.Product!.Title,
                item.User!.Name,
                item.Title,
                item.Description,
                item.Rating,
                item.Recommended,
                item.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}

