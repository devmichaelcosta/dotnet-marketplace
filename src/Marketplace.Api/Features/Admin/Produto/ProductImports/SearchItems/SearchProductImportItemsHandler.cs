using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Produto.ProductImports.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Produto.ProductImports.SearchItems;

public sealed class SearchProductImportItemsHandler(MarketplaceDbContext db)
{
    public async Task<PagedResult<ProductImportJobItemResponse>?> HandleAsync(
        SearchProductImportItemsQuery query,
        CancellationToken cancellationToken)
    {
        if (!await db.ProductImportJobs.AnyAsync(item => item.Id == query.Id, cancellationToken))
        {
            return null;
        }

        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize <= 0 ? 10 : query.PageSize, 1, 100);
        var itemsQuery = db.ProductImportJobItems
            .AsNoTracking()
            .Where(item => item.JobId == query.Id);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            itemsQuery = itemsQuery.Where(item =>
                item.Sku.Contains(query.Search) ||
                item.Title.Contains(query.Search) ||
                item.ErrorMessage.Contains(query.Search));
        }

        if (Enum.TryParse(query.Status, true, out ProductImportJobItemStatus parsedStatus))
        {
            itemsQuery = itemsQuery.Where(item => item.Status == parsedStatus);
        }

        itemsQuery = (query.Sort?.ToLowerInvariant(), query.Direction?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true) switch
        {
            ("sku", true) => itemsQuery.OrderByDescending(item => item.Sku),
            ("sku", false) => itemsQuery.OrderBy(item => item.Sku),
            ("status", true) => itemsQuery.OrderByDescending(item => item.Status),
            ("status", false) => itemsQuery.OrderBy(item => item.Status),
            ("row", true) => itemsQuery.OrderByDescending(item => item.RowNumber),
            _ => itemsQuery.OrderBy(item => item.RowNumber)
        };

        var total = await itemsQuery.CountAsync(cancellationToken);
        var items = await itemsQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(item => ProductImportJobItemResponse.From(item))
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductImportJobItemResponse>(items, total, page, pageSize);
    }
}

