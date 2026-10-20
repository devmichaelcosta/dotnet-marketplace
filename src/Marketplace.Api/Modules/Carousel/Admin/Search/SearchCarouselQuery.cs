namespace Marketplace.Api.Modules.Carousel.Admin.Search;

public sealed class SearchCarouselQuery
{
    public string? Search { get; init; }
    public string? Sort { get; init; }
    public string? Direction { get; init; }
}
