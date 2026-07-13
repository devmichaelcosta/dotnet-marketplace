using Marketplace.Api.Features.Admin.Shared;

namespace Marketplace.Tests;

public sealed class AdminUploadPolicyTests
{
    [Theory]
    [InlineData("produto.jpg", "image/jpeg")]
    [InlineData("produto.jpeg", "image/jpeg")]
    [InlineData("produto.png", "image/png")]
    [InlineData("produto.gif", "image/gif")]
    [InlineData("produto.webp", "image/webp")]
    public void Upload_policy_accepts_supported_image_extensions(string fileName, string contentType)
    {
        Assert.True(UploadPolicy.IsSupportedImage(fileName, contentType));
    }

    [Theory]
    [InlineData("", "image/png")]
    [InlineData("produto.pdf", "image/png")]
    [InlineData("produto.png", "application/octet-stream")]
    public void Upload_policy_rejects_invalid_file_names_or_content_types(string fileName, string contentType)
    {
        Assert.False(UploadPolicy.IsSupportedImage(fileName, contentType));
    }

    [Theory]
    [InlineData("categories", "categories")]
    [InlineData("categoriaS", "categories")]
    [InlineData("category", "categories")]
    [InlineData("carousel", "carousel")]
    [InlineData("destaques", "carousel")]
    [InlineData("anything-else", "products")]
    public void Upload_policy_normalizes_known_scopes(string scope, string expected)
    {
        Assert.Equal(expected, UploadPolicy.NormalizeScope(scope));
    }

    [Fact]
    public void Upload_policy_keeps_image_size_limit_at_five_megabytes()
    {
        Assert.Equal(5 * 1024 * 1024, UploadPolicy.MaxSizeBytes);
    }
}
