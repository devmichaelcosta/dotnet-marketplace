namespace Marketplace.Api.Features.Admin.Users.Search;

public sealed class SearchUsersQuery
{
    public string? Search { get; init; }
    public string? Sort { get; init; }
    public string? Direction { get; init; }
}
