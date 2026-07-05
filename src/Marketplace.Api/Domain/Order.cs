namespace Marketplace.Api.Domain;

public sealed class Order
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public int StateId { get; set; }
    public State? State { get; set; }
    public decimal Total { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string Name { get; set; } = string.Empty;
    public string CardOwnerName { get; set; } = string.Empty;
    public string ExpirationDate { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? Complement { get; set; }
    public List<OrderItem> Items { get; set; } = [];
}