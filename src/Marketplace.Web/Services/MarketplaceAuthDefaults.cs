namespace Marketplace.Web.Services;

public static class MarketplaceAuthDefaults
{
    public const string AccessTokenClaim = "marketplace:access_token";
}

public sealed record WebLoginResult(bool Succeeded, string? Message);
