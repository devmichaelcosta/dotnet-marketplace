using Marketplace.Api.Infrastructure.Security;

namespace Marketplace.Tests;

public sealed class JwtSigningKeyTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("too-short")]
    public void Create_rejects_missing_or_weak_secrets(string? secret)
    {
        var exception = Assert.Throws<InvalidOperationException>(() => JwtSigningKey.Create(secret));

        Assert.Contains("Jwt:Key", exception.Message);
    }

    [Fact]
    public void Create_accepts_secret_with_at_least_32_utf8_bytes()
    {
        var key = JwtSigningKey.Create(new string('k', JwtSigningKey.MinimumKeyLengthInBytes));

        Assert.Equal(JwtSigningKey.MinimumKeyLengthInBytes, key.Key.Length);
    }
}
