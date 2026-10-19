using Marketplace.Api.Infrastructure.Security;
using Marketplace.Api.Domain;
using Marketplace.Api.Modules.Cart.Website.Shared;
using Marketplace.Api.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Marketplace.Api.Modules.Cart.Website.Checkout;

public sealed class CheckoutHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(CheckoutRequest request, HttpContext http, CancellationToken cancellationToken)
    {
        var validation = CheckoutValidationPolicy.Validate(request);
        if (validation is not null)
        {
            return Results.ValidationProblem(validation);
        }

        var userId = http.User.GetUserId();
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var stateExists = await db.States.AnyAsync(state => state.Id == request.StateId, cancellationToken);
        if (!stateExists)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["stateId"] = ["Estado informado nao existe."]
            });
        }

        var cart = await db.GetOrCreateCartAsync(http.Request, userId, cancellationToken);
        await db.Entry(cart).Collection(item => item.Items).Query().Include(item => item.Product).LoadAsync(cancellationToken);
        if (cart.Items.Count == 0)
        {
            return Results.BadRequest(new { Message = "Carrinho vazio." });
        }

        await using IDbContextTransaction transaction = await db.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        var order = new Order
        {
            UserId = userId.Value,
            StateId = request.StateId,
            Name = request.Name!,
            Address = request.Address!,
            Neighborhood = request.Neighborhood!,
            Cep = request.Cep!,
            City = request.City!,
            Cpf = request.Cpf!,
            Complement = request.Complement,
            Total = CartPolicy.CalculateTotal(cart.Items),
            Items = cart.Items.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };

        foreach (var item in cart.Items.OrderBy(item => item.ProductId))
        {
            var affectedRows = await db.Products
                .Where(product => product.Id == item.ProductId && product.Stock >= item.Quantity)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(
                        product => product.Stock,
                        product => product.Stock - item.Quantity),
                    cancellationToken);

            if (affectedRows != 1)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Results.BadRequest(new { Message = $"Estoque insuficiente para {item.Product!.Title}." });
            }
        }

        db.Orders.Add(order);
        db.CartItems.RemoveRange(cart.Items);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Results.Created($"/api/orders/{order.Id}", new { order.Id });
    }
}
