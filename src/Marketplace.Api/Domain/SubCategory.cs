namespace Marketplace.Api.Domain;

public sealed class SubCategory
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<Product> Products { get; set; } = [];
}
