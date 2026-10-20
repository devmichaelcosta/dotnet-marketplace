using System.Text;
using Marketplace.Api.Modules.Auth.Website.Shared;
using Microsoft.AspNetCore.WebUtilities;

namespace Marketplace.Tests;

public sealed class PasswordResetLinkBuilderTests
{
    [Fact]
    public void Build_uses_configured_base_url_and_url_safe_token()
    {
        const string rawToken = "token/with+reserved=characters";

        var link = PasswordResetLinkBuilder.Build("https://shop.example.test/app/", "user+name", rawToken);
        var uri = new Uri(link);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

        Assert.Equal("https://shop.example.test/app/reset-password", uri.GetLeftPart(UriPartial.Path));
        Assert.Equal("user+name", query["login"]);
        Assert.Equal(rawToken, Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(query["token"]!)));
    }

    [Fact]
    public void Build_rejects_missing_or_non_http_public_url()
    {
        Assert.Throws<InvalidOperationException>(() => PasswordResetLinkBuilder.Build(null, "user", "token"));
        Assert.Throws<InvalidOperationException>(() => PasswordResetLinkBuilder.Build("file:///tmp/", "user", "token"));
    }
}