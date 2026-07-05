namespace Marketplace.Api.Domain;

public sealed class Product
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public int? SubCategoryId { get; set; }
    public SubCategory? SubCategory { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public bool Offer { get; set; }
    public int Stock { get; set; }
    public string Sku { get; set; } = string.Empty;
    public List<ProductImage> Images { get; set; } = [];
    public List<ProductAttributeValue> AttributeValues { get; set; } = [];
    public List<ProductRating> Ratings { get; set; } = [];
}
