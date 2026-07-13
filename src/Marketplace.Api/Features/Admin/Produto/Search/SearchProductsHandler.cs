using Marketplace.Api.Features.Admin.Produto.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Produto.Search;

public sealed class SearchProductsHandler(MarketplaceDbContext db, ProductAdminAccessPolicy accessPolicy)
{
    public async Task<SearchProductsResponse> HandleAsync(SearchProductsQuery query, HttpContext http, CancellationToken cancellationToken)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 10 : Math.Clamp(query.PageSize, 1, 100);
        var direction = string.Equals(query.Direction, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";
        var actor = accessPolicy.ResolveActor(http);

        var products = db.Products
            .Include(item => item.Images)
            .Include(item => item.User)
            .AsQueryable();

        if (actor.IsSeller && !actor.IsAdmin)
        {
            products = products.Where(item => item.UserId == actor.UserId);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            products = products.Where(item =>
                item.Title.Contains(query.Search) ||
                item.Description.Contains(query.Search) ||
                item.Sku.Contains(query.Search));
        }

        var total = await products.CountAsync(cancellationToken);
        products = (query.Sort?.ToLowerInvariant(), direction) switch
        {
            ("sku", "desc") => products.OrderByDescending(item => item.Sku),
            ("sku", _) => products.OrderBy(item => item.Sku),
            ("stock", "desc") => products.OrderByDescending(item => item.Stock),
            ("stock", _) => products.OrderBy(item => item.Stock),
            ("price", "desc") => products.OrderByDescending(item => item.Price),
            ("price", _) => products.OrderBy(item => item.Price),
            ("seller", "desc") => products.OrderByDescending(item => item.User!.Name),
            ("seller", _) => products.OrderBy(item => item.User!.Name),
            ("offer", "desc") => products.OrderByDescending(item => item.Offer),
            ("offer", _) => products.OrderBy(item => item.Offer),
            _ when direction == "desc" => products.OrderByDescending(item => item.Title),
            _ => products.OrderBy(item => item.Title)
        };

        var items = await products
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(item => new AdminProductSummary(item.Id, item.Title, item.Price, item.Stock, item.Offer, item.Sku, item.User!.Name))
            .ToArrayAsync(cancellationToken);

        return new SearchProductsResponse(items, total, page, pageSize);
    }
}
