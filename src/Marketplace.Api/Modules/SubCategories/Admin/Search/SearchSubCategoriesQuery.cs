namespace Marketplace.Api.Modules.SubCategories.Admin.Search;

public sealed class SearchSubCategoriesQuery
{
    public int? CategoryId { get; init; }
    public string? Search { get; init; }
    public string? Sort { get; init; }
    public string? Direction { get; init; }
}
