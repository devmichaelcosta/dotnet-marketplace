using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Website.Cart.DeleteItem;

public sealed class DeleteCartItemHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(int productId, HttpContext http, CancellationToken cancellationToken)
    {
        var cart = await db.GetOrCreateCartAsync(http.Request, http.User.GetUserId(), cancellationToken);
        await db.CartItems.Where(item => item.CartId == cart.Id && item.ProductId == productId).ExecuteDeleteAsync(cancellationToken);
        return Results.NoContent();
    }
}
