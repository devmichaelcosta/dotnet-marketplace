namespace Marketplace.Web.Services;

public static class MarketplaceAuthDefaults
{
    public const string AccessTokenClaim = "marketplace:access_token";
}

public sealed record WebLoginRequest(string Login, string Password);
public sealed record WebLoginResult(bool Succeeded, string? Message);
