using System.Text;
using Marketplace.Api.Modules.Auth.Website.Shared;
using Microsoft.AspNetCore.WebUtilities;

namespace Marketplace.Api.Modules.Auth.Website.PasswordRecovery.Confirm;

public sealed class ResetPasswordHandler(IPasswordResetIdentity identity)
{
    public async Task<bool> HandleAsync(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Login)
            || string.IsNullOrWhiteSpace(request.Token)
            || string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return false;
        }

        var account = await identity.FindByLoginAsync(request.Login.Trim(), cancellationToken);
        if (account is null)
        {
            return false;
        }

        string token;
        try
        {
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
        }
        catch (FormatException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }

        return await identity.ResetPasswordAsync(account.Id, token, request.NewPassword, cancellationToken);
    }
}