namespace Marketplace.Api.Features.Admin.Orders.Search;

public sealed class SearchAdminOrdersQuery
{
    public string? Search { get; init; }
    public string? Sort { get; init; }
    public string? Direction { get; init; }
}
