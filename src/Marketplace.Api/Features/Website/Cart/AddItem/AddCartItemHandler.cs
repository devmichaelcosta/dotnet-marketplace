using Marketplace.Api.Features.Website.Cart.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Website.Cart.AddItem;

public sealed class AddCartItemHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(CartItemRequest request, HttpContext http, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]> { ["quantity"] = ["Quantidade deve ser maior que zero."] });
        }

        var product = await db.Products.Include(item => item.Images).FirstOrDefaultAsync(item => item.Id == request.ProductId, cancellationToken);
        if (product is null)
        {
            return Results.NotFound();
        }

        var cart = await db.GetOrCreateCartAsync(http.Request, http.User.GetUserId(), cancellationToken);
        var item = cart.Items.FirstOrDefault(entity => entity.ProductId == product.Id);
        var newQuantity = CartPolicy.CalculateNewQuantity(item?.Quantity ?? 0, request.Quantity);
        if (!CartPolicy.HasAvailableStock(product.Stock, newQuantity))
        {
            return Results.BadRequest(new { Message = $"Estoque atual e {product.Stock}." });
        }

        if (item is null)
        {
            cart.Items.Add(new Marketplace.Api.Domain.CartItem { ProductId = product.Id, Quantity = request.Quantity, UnitPrice = product.Price });
        }
        else
        {
            item.Quantity = newQuantity;
        }

        cart.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        await db.Entry(cart).Collection(entity => entity.Items).Query().Include(entity => entity.Product!).ThenInclude(entity => entity.Images).LoadAsync(cancellationToken);
        return Results.Ok(cart.ToResponse());
    }
}
