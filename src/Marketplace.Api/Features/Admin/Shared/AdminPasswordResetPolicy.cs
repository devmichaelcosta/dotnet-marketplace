using Marketplace.Api.Features.Admin.Users.ResetPassword;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.Shared;

public static class AdminPasswordResetPolicy
{
    public const string RequiredRole = MarketplaceSeed.AdminRole;

    public static Dictionary<string, string[]> Validate(ResetUserPasswordRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            errors["password"] = ["Senha obrigatoria."];
        }

        return errors;
    }
}
