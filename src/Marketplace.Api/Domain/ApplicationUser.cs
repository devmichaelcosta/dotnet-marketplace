using Microsoft.AspNetCore.Identity;

namespace Marketplace.Api.Domain;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Cpf { get; set; }

    public Seller? Seller { get; set; }
    public List<Address> Addresses { get; set; } = [];
    public List<Product> Products { get; set; } = [];
    public List<Order> Orders { get; set; } = [];
}

