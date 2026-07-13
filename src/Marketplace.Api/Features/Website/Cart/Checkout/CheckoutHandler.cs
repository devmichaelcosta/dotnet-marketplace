using Marketplace.Api.Domain;
using Marketplace.Api.Features.Website.Cart.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Marketplace.Api.Features.Website.Cart.Checkout;

public sealed class CheckoutHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(CheckoutRequest request, HttpContext http, CancellationToken cancellationToken)
    {
        var userId = http.User.GetUserId();
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var cart = await db.GetOrCreateCartAsync(http.Request, userId, cancellationToken);
        await db.Entry(cart).Collection(item => item.Items).Query().Include(item => item.Product).LoadAsync(cancellationToken);
        if (cart.Items.Count == 0)
        {
            return Results.BadRequest(new { Message = "Carrinho vazio." });
        }

        await using IDbContextTransaction transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        foreach (var item in cart.Items)
        {
            if (item.Product!.Stock < item.Quantity)
            {
                return Results.BadRequest(new { Message = $"Estoque insuficiente para {item.Product.Title}." });
            }
        }

        var order = new Order
        {
            UserId = userId.Value,
            StateId = request.StateId,
            Name = request.Name,
            CardOwnerName = request.CardOwnerName,
            ExpirationDate = request.ExpirationDate,
            Address = request.Address,
            Neighborhood = request.Neighborhood,
            Cep = request.Cep,
            City = request.City,
            Cpf = request.Cpf,
            Complement = request.Complement,
            Total = CartPolicy.CalculateTotal(cart.Items),
            Items = cart.Items.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };

        foreach (var item in cart.Items)
        {
            item.Product!.Stock -= item.Quantity;
        }

        db.Orders.Add(order);
        db.CartItems.RemoveRange(cart.Items);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Results.Created($"/api/orders/{order.Id}", new { order.Id });
    }
}
