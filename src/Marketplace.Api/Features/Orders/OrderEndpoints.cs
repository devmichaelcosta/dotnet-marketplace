using Marketplace.Api.Features;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Orders;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").RequireAuthorization().WithTags("Orders");

        group.MapGet("/", async (HttpContext http, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var userId = http.User.GetUserId();
            var orders = await db.Orders
                .Where(order => order.UserId == userId)
                .OrderByDescending(order => order.CreatedAt)
                .Select(order => new OrderSummaryResponse(order.Id, order.Total, order.CreatedAt, order.Name, order.City))
                .ToListAsync(cancellationToken);
            return Results.Ok(orders);
        });

        group.MapGet("/{id:int}", async (int id, HttpContext http, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var userId = http.User.GetUserId();
            var order = await db.Orders
                .Include(item => item.State)
                .Include(item => item.Items).ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId, cancellationToken);
            return order is null ? Results.NotFound() : Results.Ok(OrderDetailsResponse.From(order));
        });

        var adminGroup = app.MapGroup("/api/admin/orders")
            .RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole))
            .WithTags("Admin Orders");

        adminGroup.MapGet("/", async (string? search, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var query = db.Orders.Include(order => order.User).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(order =>
                    order.Name.Contains(search) ||
                    order.City.Contains(search) ||
                    order.User!.Name.Contains(search) ||
                    order.User.UserName!.Contains(search));
            }

            var orders = await query
                .OrderByDescending(order => order.CreatedAt)
                .Take(100)
                .Select(order => new AdminOrderSummaryResponse(
                    order.Id,
                    order.Total,
                    order.CreatedAt,
                    order.Name,
                    order.City,
                    order.User!.Name,
                    order.User.UserName ?? string.Empty))
                .ToListAsync(cancellationToken);

            return Results.Ok(orders);
        });

        adminGroup.MapGet("/{id:int}", async (int id, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var order = await db.Orders
                .Include(item => item.User)
                .Include(item => item.State)
                .Include(item => item.Items).ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
            return order is null ? Results.NotFound() : Results.Ok(AdminOrderDetailsResponse.From(order));
        });

        return app;
    }
}

public sealed record OrderSummaryResponse(int Id, decimal Total, DateTimeOffset CreatedAt, string Name, string City);
public sealed record AdminOrderSummaryResponse(
    int Id,
    decimal Total,
    DateTimeOffset CreatedAt,
    string Name,
    string City,
    string UserName,
    string Login);

public sealed record OrderDetailsResponse(
    int Id,
    decimal Total,
    DateTimeOffset CreatedAt,
    string Name,
    string Address,
    string Neighborhood,
    string Cep,
    string City,
    string? Complement,
    string State,
    OrderItemResponse[] Items)
{
    public static OrderDetailsResponse From(Domain.Order order) =>
        new(
            order.Id,
            order.Total,
            order.CreatedAt,
            order.Name,
            order.Address,
            order.Neighborhood,
            order.Cep,
            order.City,
            order.Complement,
            order.State?.Abbreviation ?? string.Empty,
            order.Items.Select(item => new OrderItemResponse(
                item.ProductId,
                item.Product?.Title ?? string.Empty,
                item.Quantity,
                item.UnitPrice,
                item.Quantity * item.UnitPrice)).ToArray());
}

public sealed record OrderItemResponse(int ProductId, string Title, int Quantity, decimal UnitPrice, decimal SubTotal);

public sealed record AdminOrderDetailsResponse(
    int Id,
    decimal Total,
    DateTimeOffset CreatedAt,
    string Name,
    string Login,
    string Address,
    string Neighborhood,
    string Cep,
    string City,
    string? Complement,
    string State,
    OrderItemResponse[] Items)
{
    public static AdminOrderDetailsResponse From(Domain.Order order) =>
        new(
            order.Id,
            order.Total,
            order.CreatedAt,
            order.Name,
            order.User?.UserName ?? string.Empty,
            order.Address,
            order.Neighborhood,
            order.Cep,
            order.City,
            order.Complement,
            order.State?.Abbreviation ?? string.Empty,
            order.Items.Select(item => new OrderItemResponse(
                item.ProductId,
                item.Product?.Title ?? string.Empty,
                item.Quantity,
                item.UnitPrice,
                item.Quantity * item.UnitPrice)).ToArray());
}
