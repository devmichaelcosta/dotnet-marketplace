namespace Marketplace.Api.Features.Admin.Attributes.Search;

public sealed class SearchAttributesQuery
{
    public string? Search { get; init; }
    public string? Sort { get; init; }
    public string? Direction { get; init; }
}
