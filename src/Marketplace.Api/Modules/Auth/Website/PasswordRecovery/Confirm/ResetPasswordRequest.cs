namespace Marketplace.Api.Modules.Auth.Website.PasswordRecovery.Confirm;

public sealed record ResetPasswordRequest(string? Login, string? Token, string? NewPassword);