namespace Marketplace.Api.Features.Admin.Orders.GetById;

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
    Marketplace.Api.Features.Website.Orders.GetById.OrderItemResponse[] Items)
{
    public static AdminOrderDetailsResponse From(Marketplace.Api.Domain.Order order) =>
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
            order.Items.Select(item => new Marketplace.Api.Features.Website.Orders.GetById.OrderItemResponse(
                item.ProductId,
                item.Product?.Title ?? string.Empty,
                item.Quantity,
                item.UnitPrice,
                item.Quantity * item.UnitPrice)).ToArray());
}
