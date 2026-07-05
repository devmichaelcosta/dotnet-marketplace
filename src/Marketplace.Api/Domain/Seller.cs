namespace Marketplace.Api.Domain;

public sealed class Seller
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public int? Age { get; set; }
    public string? Email { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Website { get; set; }
    public string? Company { get; set; }
    public string? Cnpj { get; set; }
    public string? BranchOfActivity { get; set; }
    public string? FantasyName { get; set; }
}
