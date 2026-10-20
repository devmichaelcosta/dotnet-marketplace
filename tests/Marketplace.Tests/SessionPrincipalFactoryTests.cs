using System.Security.Claims;
using Marketplace.Api.Modules.Auth.Website.Login;
using Marketplace.Api.Modules.Auth.Website.Session;

namespace Marketplace.Tests;

public sealed class SessionPrincipalFactoryTests
{
    [Fact]
    public void Create_maps_api_login_to_web_session_claims()
    {
        var userId = Guid.NewGuid();
        var login = new LoginResponse(
            "server-only-jwt",
            new LoginUserResponse(userId, "michael", "Michael", ["admin"]));

        var principal = SessionPrincipalFactory.Create(login);

        Assert.True(principal.Identity?.IsAuthenticated);
        Assert.Equal("michael", principal.Identity?.Name);
        Assert.True(principal.IsInRole("admin"));
        Assert.Equal("server-only-jwt", principal.FindFirst(SessionPrincipalFactory.AccessTokenClaim)?.Value);
        Assert.Equal(userId.ToString(), principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    }

}
