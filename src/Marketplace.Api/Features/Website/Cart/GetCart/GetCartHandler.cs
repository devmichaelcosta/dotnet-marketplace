using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Website.Cart.GetCart;

public sealed class GetCartHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(HttpContext http, CancellationToken cancellationToken)
    {
        var cart = await db.GetOrCreateCartAsync(http.Request, http.User.GetUserId(), cancellationToken);

        await db.Entry(cart)
            .Collection(item => item.Items).Query()
            .Include(item => item.Product!).ThenInclude(item => item.Images)
            .LoadAsync(cancellationToken);

        return Results.Ok(cart.ToResponse());
    }
}
