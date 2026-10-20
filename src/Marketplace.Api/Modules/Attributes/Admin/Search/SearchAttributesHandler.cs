using Marketplace.Api.Shared.Admin;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Modules.Attributes.Admin.Search;

public sealed class SearchAttributesHandler(MarketplaceDbContext db)
{
    public async Task<List<AttributeResponse>> HandleAsync(SearchAttributesQuery query, CancellationToken cancellationToken)
    {
        return await AdminListQueryPolicy.ApplyAttributeSort(
                db.Attributes.Where(item => string.IsNullOrWhiteSpace(query.Search) || item.Name.Contains(query.Search)),
                query.Sort,
                query.Direction)
            .Select(item => new AttributeResponse(item.Id, item.Name))
            .ToListAsync(cancellationToken);
    }
}
