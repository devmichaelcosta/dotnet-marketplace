using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Website.Orders.Search;

public sealed class SearchOrdersHandler(MarketplaceDbContext db)
{
    public async Task<IReadOnlyList<OrderSummaryResponse>> HandleAsync(HttpContext http, CancellationToken cancellationToken)
    {
        var userId = http.User.GetUserId();
        return await db.Orders
            .Where(order => order.UserId == userId)
            .OrderByDescending(order => order.CreatedAt)
            .Select(order => new OrderSummaryResponse(order.Id, order.Total, order.CreatedAt, order.Name, order.City))
            .ToListAsync(cancellationToken);
    }
}
