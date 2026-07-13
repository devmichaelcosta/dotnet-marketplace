using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.Users;

public static class UserRoleNormalizer
{
    public static string NormalizeRole(string? role) =>
        role is MarketplaceSeed.AdminRole or MarketplaceSeed.SellerRole or MarketplaceSeed.CustomerRole
            ? role
            : MarketplaceSeed.CustomerRole;
}
