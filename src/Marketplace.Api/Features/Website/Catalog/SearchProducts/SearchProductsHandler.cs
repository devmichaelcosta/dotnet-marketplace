using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Website.Catalog.SearchProducts;

public sealed class SearchProductsHandler(MarketplaceDbContext db)
{
    public async Task<SearchProductsResponse> HandleAsync(SearchProductsQuery query, CancellationToken cancellationToken)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var products = db.Products.Include(product => product.Images).Include(product => product.User).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            products = products.Where(product =>
                product.Title.Contains(query.Search) ||
                product.Description.Contains(query.Search) ||
                product.Sku.Contains(query.Search));
        }

        if (query.SubCategoryId is not null)
        {
            products = products.Where(product => product.SubCategoryId == query.SubCategoryId);
        }
        else if (query.CategoryId is not null)
        {
            products = products.Where(product => product.SubCategory != null && product.SubCategory.CategoryId == query.CategoryId);
        }

        var total = await products.CountAsync(cancellationToken);
        var items = await products.OrderBy(product => product.Title)
            .Skip((page - 1) * 12)
            .Take(12)
            .Select(product => ProductSummary.From(product))
            .ToListAsync(cancellationToken);

        return new SearchProductsResponse(items, total, page, 12);
    }
}
