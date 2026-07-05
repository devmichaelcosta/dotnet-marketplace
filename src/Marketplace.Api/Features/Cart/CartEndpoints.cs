using Marketplace.Api.Domain;
using Marketplace.Api.Features;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Cart;

public static class CartEndpoints
{
    public static IEndpointRouteBuilder MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cart").WithTags("Cart");

        group.MapGet("/", async (HttpContext http, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var cart = await db.GetOrCreateCartAsync(http.Request, http.User.GetUserId(), cancellationToken);
            await db.Entry(cart).Collection(item => item.Items).Query().Include(item => item.Product!).ThenInclude(item => item.Images).LoadAsync(cancellationToken);
            return Results.Ok(cart.ToResponse());
        });

        group.MapPost("/items", async (CartItemRequest request, HttpContext http, MarketplaceDbContext db, CancellationToken cancellationToken) =>
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
            var item = cart.Items.FirstOrDefault(item => item.ProductId == product.Id);
            var newQuantity = CartPolicy.CalculateNewQuantity(item?.Quantity ?? 0, request.Quantity);
            if (!CartPolicy.HasAvailableStock(product.Stock, newQuantity))
            {
                return Results.BadRequest(new { Message = $"Estoque atual e {product.Stock}." });
            }

            if (item is null)
            {
                cart.Items.Add(new CartItem { ProductId = product.Id, Quantity = request.Quantity, UnitPrice = product.Price });
            }
            else
            {
                item.Quantity = newQuantity;
            }

            cart.UpdatedAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(cancellationToken);
            await db.Entry(cart).Collection(entity => entity.Items).Query().Include(entity => entity.Product!).ThenInclude(entity => entity.Images).LoadAsync(cancellationToken);
            return Results.Ok(cart.ToResponse());
        });

        group.MapPut("/items/{productId:int}", async (int productId, UpdateCartItemRequest request, HttpContext http, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            if (request.Quantity <= 0)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["quantity"] = ["Quantidade deve ser maior que zero."] });
            }

            var cart = await db.GetOrCreateCartAsync(http.Request, http.User.GetUserId(), cancellationToken);
            var item = cart.Items.FirstOrDefault(item => item.ProductId == productId);
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
        });

        group.MapDelete("/items/{productId:int}", async (int productId, HttpContext http, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var cart = await db.GetOrCreateCartAsync(http.Request, http.User.GetUserId(), cancellationToken);
            await db.CartItems.Where(item => item.CartId == cart.Id && item.ProductId == productId).ExecuteDeleteAsync(cancellationToken);
            return Results.NoContent();
        });

        group.MapPost("/checkout", async (CheckoutRequest request, HttpContext http, MarketplaceDbContext db, CancellationToken cancellationToken) =>
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

            await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

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
        }).RequireAuthorization();

        return app;
    }
}

public static class CartPolicy
{
    public static int CalculateNewQuantity(int currentQuantity, int requestedQuantity) => currentQuantity + requestedQuantity;

    public static bool HasAvailableStock(int stock, int requestedQuantity) => stock >= requestedQuantity;

    public static decimal CalculateTotal(IEnumerable<CartItem> items) => items.Sum(item => item.UnitPrice * item.Quantity);
}

public sealed record CartItemRequest(int ProductId, int Quantity);
public sealed record UpdateCartItemRequest(int Quantity);
public sealed record CheckoutRequest(
    string Name,
    string CardOwnerName,
    string ExpirationDate,
    string Address,
    string Neighborhood,
    string Cep,
    string City,
    string Cpf,
    int StateId,
    string? Complement);
