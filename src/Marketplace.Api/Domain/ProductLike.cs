namespace Marketplace.Api.Domain;

public sealed class ProductLike
{
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
