using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Marketplace.Api.Infrastructure.Security;

public static class JwtSigningKey
{
    public const int MinimumKeyLengthInBytes = 32;

    public static SymmetricSecurityKey Create(string? configuredKey)
    {
        if (string.IsNullOrWhiteSpace(configuredKey))
        {
            throw new InvalidOperationException(
                "Configuration 'Jwt:Key' is required. Provide a randomly generated secret of at least 32 bytes.");
        }

        var keyBytes = Encoding.UTF8.GetBytes(configuredKey);
        if (keyBytes.Length < MinimumKeyLengthInBytes)
        {
            throw new InvalidOperationException(
                "Configuration 'Jwt:Key' must contain at least 32 bytes. Provide a randomly generated secret.");
        }

        return new SymmetricSecurityKey(keyBytes);
    }
}
