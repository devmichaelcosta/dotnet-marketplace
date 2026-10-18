namespace Marketplace.Api.Modules.Auth.Website.Shared;

public interface IPasswordResetIdentity
{
    Task<PasswordResetAccount?> FindByLoginAsync(string login, CancellationToken cancellationToken);
    Task<string> GenerateTokenAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> ResetPasswordAsync(Guid userId, string token, string password, CancellationToken cancellationToken);
}
