namespace Marketplace.Api.Modules.Orders.Website.Search;

public sealed record OrderSummaryResponse(int Id, decimal Total, DateTimeOffset CreatedAt, string Name, string City);
