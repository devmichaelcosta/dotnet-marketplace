using Marketplace.Api.Modules.Auth.Website.Shared;

namespace Marketplace.Tests;

public sealed class FakePasswordResetIdentity : IPasswordResetIdentity
{
    public PasswordResetAccount? Account { get; set; }
    public string GeneratedToken { get; set; } = "reset/token+value";
    public bool AcceptResetToken { get; set; } = true;
    public bool TokenConsumed { get; private set; }

    public Task<PasswordResetAccount?> FindByLoginAsync(string login, CancellationToken cancellationToken) =>
        Task.FromResult(Account is not null && string.Equals(Account.Login, login, StringComparison.OrdinalIgnoreCase)
            ? Account
            : null);

    public Task<string> GenerateTokenAsync(Guid userId, CancellationToken cancellationToken) =>
        Task.FromResult(GeneratedToken);

    public Task<bool> ResetPasswordAsync(Guid userId, string token, string password, CancellationToken cancellationToken)
    {
        var succeeded = Account?.Id == userId
            && AcceptResetToken
            && !TokenConsumed
            && string.Equals(token, GeneratedToken, StringComparison.Ordinal);
        if (succeeded)
        {
            TokenConsumed = true;
        }

        return Task.FromResult(succeeded);
    }
}