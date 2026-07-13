using Marketplace.Api.Features.Website.Cart.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Website.Cart.UpdateItem;

public sealed class UpdateCartItemHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(int productId, UpdateCartItemRequest request, HttpContext http, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]> { ["quantity"] = ["Quantidade deve ser maior que zero."] });
        }

        var cart = await db.GetOrCreateCartAsync(http.Request, http.User.GetUserId(), cancellationToken);
        var item = cart.Items.FirstOrDefault(entity => entity.ProductId == productId);
        if (item is null)
        {
            return Results.NotFound();
        }

        var stock = await db.Products.Where(product => product.Id == productId).Select(product => product.Stock).FirstAsync(cancellationToken);
        if (!CartPolicy.HasAvailableStock(stock, request.Quantity))
        {
            return Results.BadRequest(new { Message = $"Estoque atual e {stock}." });
        }

        item.Quantity = request.Quantity;
        cart.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return Results.NoContent();
    }
}
