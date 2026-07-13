using Marketplace.Api.Features.Admin.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.SubCategories.Search;

public sealed class SearchSubCategoriesHandler(MarketplaceDbContext db)
{
    public async Task<List<SubCategoryResponse>> HandleAsync(SearchSubCategoriesQuery query, CancellationToken cancellationToken)
    {
        var subCategories = db.SubCategories.Include(item => item.Category).AsQueryable();
        if (query.CategoryId is not null)
        {
            subCategories = subCategories.Where(item => item.CategoryId == query.CategoryId);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            subCategories = subCategories.Where(item =>
                item.Title.Contains(query.Search) ||
                item.Category!.Title.Contains(query.Search));
        }

        subCategories = AdminListQueryPolicy.ApplySubCategorySort(subCategories, query.Sort, query.Direction);

        return await subCategories
            .Select(item => SubCategoryResponse.From(item))
            .ToListAsync(cancellationToken);
    }
}
