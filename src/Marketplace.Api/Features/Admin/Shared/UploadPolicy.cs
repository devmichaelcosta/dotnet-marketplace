namespace Marketplace.Api.Features.Admin.Shared;

public static class UploadPolicy
{
    public const long MaxSizeBytes = 5 * 1024 * 1024;

    public static bool IsSupportedImage(string fileName, string contentType)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return !string.IsNullOrWhiteSpace(fileName)
            && (extension is ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp")
            && contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    }

    public static string NormalizeScope(string scope) =>
        scope.Trim().ToLowerInvariant() switch
        {
            "categories" or "category" or "categorias" => "categories",
            "carousel" or "destaques" => "carousel",
            _ => "products"
        };
}
