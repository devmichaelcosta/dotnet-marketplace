namespace Marketplace.Api.Modules.Auth.Website.Session;

public sealed record SessionLoginResponse(bool Succeeded, string? Message);
