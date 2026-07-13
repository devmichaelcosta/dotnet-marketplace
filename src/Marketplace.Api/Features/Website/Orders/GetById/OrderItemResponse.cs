namespace Marketplace.Api.Features.Website.Orders.GetById;

public sealed record OrderItemResponse(int ProductId, string Title, int Quantity, decimal UnitPrice, decimal SubTotal);
