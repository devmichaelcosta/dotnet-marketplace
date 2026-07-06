using Marketplace.Api.Features.Users;

namespace Marketplace.Tests;

public sealed class UserPolicyTests
{
    [Fact]
    public void Profile_policy_accepts_valid_profile_data()
    {
        var result = ProfilePolicy.Validate(new ProfileRequest(
            "Maria",
            "Silva",
            "123.456.789-09",
            Array.Empty<AddressRequest>()));

        Assert.Null(result);
    }

    [Fact]
    public void Profile_policy_rejects_missing_name_and_last_name()
    {
        var result = ProfilePolicy.Validate(new ProfileRequest(
            string.Empty,
            " ",
            "12345678909",
            Array.Empty<AddressRequest>()));

        Assert.NotNull(result);
    }

    [Fact]
    public void Profile_policy_normalizes_cpf_digits()
    {
        Assert.Equal("12345678909", ProfilePolicy.NormalizeDocument("123.456.789-09"));
    }
}
