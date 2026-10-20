using Marketplace.Api.Infrastructure.Persistence;
using Marketplace.Api.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Modules.Orders.Website.GetById;

public sealed class GetOrderByIdHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(int id, HttpContext http, CancellationToken cancellationToken)
    {
        var userId = http.User.GetUserId();
        var order = await db.Orders
            .AsNoTracking()
            .Include(item => item.State)
            .Include(item => item.Items).ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId, cancellationToken);

        return order is null ? Results.NotFound() : Results.Ok(OrderDetailsResponse.From(order));
    }
}
