namespace Marketplace.Api.Features.Website.Orders.GetById;

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
    public static OrderDetailsResponse From(Marketplace.Api.Domain.Order order) =>
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
