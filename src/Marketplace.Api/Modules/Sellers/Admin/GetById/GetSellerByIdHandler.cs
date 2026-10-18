using Marketplace.Api.Modules.Sellers.Admin.Search;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Modules.Sellers.Admin.GetById;

public sealed class GetSellerByIdHandler(MarketplaceDbContext db)
{
    public async Task<SellerResponse?> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var seller = await db.Sellers.Include(item => item.User).FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return seller is null ? null : SellerResponse.From(seller);
    }
}
