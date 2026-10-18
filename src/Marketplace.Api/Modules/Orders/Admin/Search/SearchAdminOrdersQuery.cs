namespace Marketplace.Api.Modules.Orders.Admin.Search;

public sealed class SearchAdminOrdersQuery
{
    public string? Search { get; init; }
    public string? Sort { get; init; }
    public string? Direction { get; init; }
}
