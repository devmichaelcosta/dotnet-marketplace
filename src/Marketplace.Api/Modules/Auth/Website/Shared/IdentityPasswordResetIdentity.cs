using Marketplace.Api.Domain;
using Microsoft.AspNetCore.Identity;

namespace Marketplace.Api.Modules.Auth.Website.Shared;

public sealed class IdentityPasswordResetIdentity(UserManager<ApplicationUser> userManager) : IPasswordResetIdentity
{
    public async Task<PasswordResetAccount?> FindByLoginAsync(string login, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await userManager.FindByNameAsync(login);
        return user is null
            ? null
            : new PasswordResetAccount(user.Id, user.UserName ?? login, user.Email);
    }

    public async Task<string> GenerateTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            throw new InvalidOperationException("The account is no longer available for password recovery.");
        }

        return await userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<bool> ResetPasswordAsync(
        Guid userId,
        string token,
        string password,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return false;
        }

        var result = await userManager.ResetPasswordAsync(user, token, password);
        return result.Succeeded;
    }
}
