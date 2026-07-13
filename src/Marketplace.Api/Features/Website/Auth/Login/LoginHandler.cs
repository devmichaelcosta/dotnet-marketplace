using Marketplace.Api.Domain;
using Marketplace.Api.Features.Website.Auth.Shared;
using Marketplace.Api.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Website.Auth.Login;

public sealed class LoginHandler(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    TokenService tokenService)
{
    public async Task<IResult> HandleAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(item => item.UserName == request.Login, cancellationToken);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return Results.Unauthorized();
        }

        var roles = await userManager.GetRolesAsync(user);
        var token = await tokenService.CreateTokenAsync(user);
        return Results.Ok(new { Token = token, User = new { user.Id, user.UserName, user.Name, Roles = roles } });
    }
}
