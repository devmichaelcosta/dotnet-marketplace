namespace Marketplace.Api.Domain;

public sealed class Cart
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public string AnonymousKey { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public List<CartItem> Items { get; set; } = [];
}
