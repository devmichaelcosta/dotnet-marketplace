using Marketplace.Api.Domain;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.Users.Search;

public sealed record UserResponse(Guid Id, string Login, string Name, string LastName, string? Cpf, string Role)
{
    public static UserResponse From(ApplicationUser user, string[] roles) =>
        new(user.Id, user.UserName ?? string.Empty, user.Name, user.LastName, user.Cpf, roles.FirstOrDefault() ?? MarketplaceSeed.CustomerRole);
}
