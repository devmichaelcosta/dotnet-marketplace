using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Website.Orders.GetById;

public sealed class GetOrderByIdHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(int id, HttpContext http, CancellationToken cancellationToken)
    {
        var userId = http.User.GetUserId();
        var order = await db.Orders
            .Include(item => item.State)
            .Include(item => item.Items).ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId, cancellationToken);

        return order is null ? Results.NotFound() : Results.Ok(OrderDetailsResponse.From(order));
    }
}
