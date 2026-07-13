namespace Marketplace.Api.Features.Admin.Sellers.Create;

public sealed record CreateSellerRequest(
    string Name,
    string LastName,
    string Login,
    string Password,
    string? Cpf,
    int? Age,
    string? Email,
    DateOnly? DateOfBirth,
    string? Website,
    string? Company,
    string? Cnpj,
    string? BranchOfActivity,
    string? FantasyName);
