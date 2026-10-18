namespace Marketplace.Api.Modules.Auth.Website.Shared;

public sealed class PasswordResetOptions
{
    public const string SectionName = "PasswordReset";

    public string? PublicWebBaseUrl { get; init; }
}
