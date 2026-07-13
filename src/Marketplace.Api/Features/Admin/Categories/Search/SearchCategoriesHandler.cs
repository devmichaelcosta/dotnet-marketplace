using Marketplace.Api.Features.Admin.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Categories.Search;

public sealed class SearchCategoriesHandler(MarketplaceDbContext db)
{
    public async Task<List<CategoryResponse>> HandleAsync(SearchCategoriesQuery query, CancellationToken cancellationToken)
    {
        var categories = db.Categories
            .Include(item => item.SubCategories)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            categories = categories.Where(item => item.Title.Contains(query.Search));
        }

        categories = AdminListQueryPolicy.ApplyCategorySort(categories, query.Sort, query.Direction);

        return await categories
            .Select(item => CategoryResponse.From(item))
            .ToListAsync(cancellationToken);
    }
}
