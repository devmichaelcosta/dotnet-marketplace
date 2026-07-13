namespace Marketplace.Api.Features.Admin.Sellers.Update;

public sealed record UpdateSellerRequest(
    string Name,
    string LastName,
    string? Cpf,
    int? Age,
    string? Email,
    DateOnly? DateOfBirth,
    string? Website,
    string? Company,
    string? Cnpj,
    string? BranchOfActivity,
    string? FantasyName);
