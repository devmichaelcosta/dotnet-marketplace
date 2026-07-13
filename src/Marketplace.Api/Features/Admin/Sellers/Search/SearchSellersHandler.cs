using Marketplace.Api.Features.Admin.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Sellers.Search;

public sealed class SearchSellersHandler(MarketplaceDbContext db)
{
    public async Task<List<SellerResponse>> HandleAsync(SearchSellersQuery query, CancellationToken cancellationToken)
    {
        return await AdminListQueryPolicy.ApplySellerSort(
                db.Sellers
                    .Include(item => item.User)
                    .Where(item =>
                        string.IsNullOrWhiteSpace(query.Search) ||
                        item.User!.Name.Contains(query.Search) ||
                        item.User.LastName.Contains(query.Search) ||
                        (item.Email != null && item.Email.Contains(query.Search)) ||
                        (item.Company != null && item.Company.Contains(query.Search)) ||
                        (item.FantasyName != null && item.FantasyName.Contains(query.Search)) ||
                        (item.Cnpj != null && item.Cnpj.Contains(query.Search))),
                query.Sort,
                query.Direction)
            .Select(item => SellerResponse.From(item))
            .ToListAsync(cancellationToken);
    }
}
