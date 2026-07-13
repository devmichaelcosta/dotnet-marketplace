using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Users.Search;
using Microsoft.AspNetCore.Identity;

namespace Marketplace.Api.Features.Admin.Users.GetById;

public sealed class GetUserByIdHandler(UserManager<ApplicationUser> userManager)
{
    public async Task<UserResponse?> HandleAsync(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return null;
        }

        var roles = await userManager.GetRolesAsync(user);
        return UserResponse.From(user, roles.ToArray());
    }
}
