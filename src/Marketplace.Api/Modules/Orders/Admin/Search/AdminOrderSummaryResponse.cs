namespace Marketplace.Api.Modules.Orders.Admin.Search;

public sealed record AdminOrderSummaryResponse(
    int Id,
    decimal Total,
    DateTimeOffset CreatedAt,
    string Name,
    string City,
    string UserName,
    string Login);
