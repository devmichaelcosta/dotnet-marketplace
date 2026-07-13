using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Orders.GetById;

public sealed class GetAdminOrderByIdHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(int id, CancellationToken cancellationToken)
    {
        var order = await db.Orders
            .Include(item => item.User)
            .Include(item => item.State)
            .Include(item => item.Items).ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return order is null ? Results.NotFound() : Results.Ok(AdminOrderDetailsResponse.From(order));
    }
}
