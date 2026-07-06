using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Marketplace.Web.Services;

namespace Marketplace.Tests;

public sealed class MarketplaceAuthServiceTests
{
    [Fact]
    public async Task AuthenticateAsync_creates_server_principal_from_api_login()
    {
        var userId = Guid.NewGuid();
        using var client = new HttpClient(new StubHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new LoginResponse(
                    "server-only-jwt",
                    new LoginUser(userId, "michael", "Michael", ["admin"])))
            }))
        {
            BaseAddress = new Uri("http://api.local/")
        };
        var service = new MarketplaceAuthService(client);

        var principal = await service.AuthenticateAsync(new WebLoginRequest("michael", "Password1"));

        Assert.NotNull(principal);
        Assert.True(principal.Identity?.IsAuthenticated);
        Assert.Equal("michael", principal.Identity?.Name);
        Assert.True(principal.IsInRole("admin"));
        Assert.Equal("server-only-jwt", principal.FindFirst(MarketplaceAuthDefaults.AccessTokenClaim)?.Value);
        Assert.Equal(userId.ToString(), principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    }

    [Fact]
    public async Task AuthenticateAsync_returns_null_for_invalid_credentials()
    {
        using var client = new HttpClient(new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.Unauthorized)))
        {
            BaseAddress = new Uri("http://api.local/")
        };
        var service = new MarketplaceAuthService(client);

        var principal = await service.AuthenticateAsync(new WebLoginRequest("michael", "wrong"));

        Assert.Null(principal);
    }

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> handle) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(handle(request));
    }
}
