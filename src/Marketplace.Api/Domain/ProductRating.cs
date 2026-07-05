namespace Marketplace.Api.Domain;

public sealed class ProductRating
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Recommended { get; set; }
    public string Rating { get; set; } = string.Empty;
    public bool Approved { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
