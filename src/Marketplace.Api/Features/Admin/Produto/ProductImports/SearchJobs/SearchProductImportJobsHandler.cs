using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Produto.ProductImports.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Produto.ProductImports.SearchJobs;

public sealed class SearchProductImportJobsHandler(MarketplaceDbContext db)
{
    public async Task<PagedResult<ProductImportJobListItem>> HandleAsync(
        SearchProductImportJobsQuery query,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize <= 0 ? 10 : query.PageSize, 1, 100);
        var jobs = db.ProductImportJobs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            jobs = jobs.Where(job =>
                job.OriginalFileName.Contains(query.Search) ||
                job.ImportedByName.Contains(query.Search) ||
                job.SummaryMessage.Contains(query.Search));
        }

        if (Enum.TryParse(query.Status, true, out ProductImportJobStatus parsedStatus))
        {
            jobs = jobs.Where(job => job.Status == parsedStatus);
        }

        jobs = (query.Sort?.ToLowerInvariant(), query.Direction?.Equals("asc", StringComparison.OrdinalIgnoreCase) == true) switch
        {
            ("status", true) => jobs.OrderBy(job => job.Status),
            ("status", false) => jobs.OrderByDescending(job => job.Status),
            ("user", true) => jobs.OrderBy(job => job.ImportedByName),
            ("user", false) => jobs.OrderByDescending(job => job.ImportedByName),
            ("duration", true) => jobs.OrderBy(job => job.DurationMs),
            ("duration", false) => jobs.OrderByDescending(job => job.DurationMs),
            ("skus", true) => jobs.OrderBy(job => job.SkuCount),
            ("skus", false) => jobs.OrderByDescending(job => job.SkuCount),
            ("errors", true) => jobs.OrderBy(job => job.ErrorCount),
            ("errors", false) => jobs.OrderByDescending(job => job.ErrorCount),
            ("created", true) => jobs.OrderBy(job => job.CreatedAt),
            _ => jobs.OrderByDescending(job => job.CreatedAt)
        };

        var total = await jobs.CountAsync(cancellationToken);
        var items = await jobs
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(job => ProductImportJobListItem.From(job))
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductImportJobListItem>(items, total, page, pageSize);
    }
}

