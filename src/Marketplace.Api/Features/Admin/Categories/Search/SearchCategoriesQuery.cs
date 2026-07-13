namespace Marketplace.Api.Features.Admin.Categories.Search;

public sealed class SearchCategoriesQuery
{
    public string? Search { get; init; }
    public string? Sort { get; init; }
    public string? Direction { get; init; }
}
