using Marketplace.Api.Domain;

namespace Marketplace.Api.Features.Website.Users.Shared;

public sealed record ProfileResponse(Guid Id, string Login, string Name, string LastName, string? Cpf, string[] Roles, AddressResponse[] Addresses)
{
    public static ProfileResponse From(ApplicationUser user, string[] roles) =>
        new(
            user.Id,
            user.UserName ?? string.Empty,
            user.Name,
            user.LastName,
            user.Cpf,
            roles,
            user.Addresses.Select(AddressResponse.From).ToArray());
}
