namespace Marketplace.Api.Domain;

public sealed class Category
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Image { get; set; }
    public List<SubCategory> SubCategories { get; set; } = [];
}
