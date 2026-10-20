namespace Marketplace.Api.Modules.Orders.Website.GetById;

public sealed record OrderItemResponse(int ProductId, string Title, int Quantity, decimal UnitPrice, decimal SubTotal);
