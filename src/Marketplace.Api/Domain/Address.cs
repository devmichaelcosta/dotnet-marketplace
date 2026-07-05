namespace Marketplace.Api.Domain;

public sealed class Address
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public int StateId { get; set; }
    public State? State { get; set; }
    public string Street { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Complement { get; set; }
}
