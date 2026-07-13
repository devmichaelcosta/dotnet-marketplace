using Marketplace.Api.Domain;

namespace Marketplace.Api.Features.Admin.Sellers.Search;

public sealed record SellerResponse(
    Guid Id,
    Guid UserId,
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
    string? FantasyName)
{
    public static SellerResponse From(Seller seller) =>
        new(
            seller.Id,
            seller.UserId,
            seller.User?.Name ?? string.Empty,
            seller.User?.LastName ?? string.Empty,
            seller.User?.Cpf,
            seller.Age,
            seller.Email,
            seller.DateOfBirth,
            seller.Website,
            seller.Company,
            seller.Cnpj,
            seller.BranchOfActivity,
            seller.FantasyName);
}
