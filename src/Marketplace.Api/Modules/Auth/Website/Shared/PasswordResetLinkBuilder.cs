using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace Marketplace.Api.Modules.Auth.Website.Shared;

public static class PasswordResetLinkBuilder
{
    public static string Build(string? publicWebBaseUrl, string login, string token)
    {
        if (!Uri.TryCreate(publicWebBaseUrl, UriKind.Absolute, out var baseUri)
            || (baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException("Password reset public URL is not configured with an absolute HTTP(S) URL.");
        }

        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var baseUrl = publicWebBaseUrl!.EndsWith("/", StringComparison.Ordinal)
            ? publicWebBaseUrl
            : $"{publicWebBaseUrl}/";
        var relativeUrl = $"reset-password?login={Uri.EscapeDataString(login)}&token={Uri.EscapeDataString(encodedToken)}";
        return new Uri(new Uri(baseUrl, UriKind.Absolute), relativeUrl).ToString();
    }
}