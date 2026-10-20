namespace Marketplace.Api.Modules.Auth.Website.Shared;

public sealed record PasswordResetAccount(Guid Id, string Login, string? Email);
