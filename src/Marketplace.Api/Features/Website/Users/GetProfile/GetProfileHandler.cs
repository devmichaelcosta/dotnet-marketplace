using Marketplace.Api.Features.Website.Users.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Website.Users.GetProfile;

public sealed class GetProfileHandler(
    UserManager<Marketplace.Api.Domain.ApplicationUser> userManager,
    MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(HttpContext http, CancellationToken cancellationToken)
    {
        var userId = http.User.GetUserId();
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var user = await db.Users
            .Include(item => item.Addresses).ThenInclude(item => item.State)
            .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound();
        }

        var roles = await userManager.GetRolesAsync(user);
        return Results.Ok(ProfileResponse.From(user, roles.ToArray()));
    }
}
