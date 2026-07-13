using Marketplace.Api.Features.Admin.Shared;
using Marketplace.Api.Features.Admin.Users.ResetPassword;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Tests;

public sealed class AdminPasswordResetTests
{
    [Fact]
    public void Reset_password_policy_requires_admin_role()
    {
        Assert.Equal(MarketplaceSeed.AdminRole, AdminPasswordResetPolicy.RequiredRole);
    }

    [Fact]
    public void Reset_password_fails_fast_without_password()
    {
        var errors = AdminPasswordResetPolicy.Validate(new ResetUserPasswordRequest(string.Empty));

        Assert.True(errors.ContainsKey("password"));
    }
}
