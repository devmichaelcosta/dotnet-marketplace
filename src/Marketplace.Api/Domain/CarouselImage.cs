namespace Marketplace.Api.Domain;

public sealed class CarouselImage
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

