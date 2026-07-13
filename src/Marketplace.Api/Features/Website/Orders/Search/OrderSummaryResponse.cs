namespace Marketplace.Api.Features.Website.Orders.Search;

public sealed record OrderSummaryResponse(int Id, decimal Total, DateTimeOffset CreatedAt, string Name, string City);
