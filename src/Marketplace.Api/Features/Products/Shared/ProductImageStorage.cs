namespace Marketplace.Api.Features.Products;

internal static class ProductImageStorage
{
    public static string? NormalizeFileName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Replace('\\', '/').Trim();
        var fileName = Path.GetFileName(normalized);
        return string.IsNullOrWhiteSpace(fileName) ? null : fileName;
    }

    public static string[] NormalizeFileNames(IEnumerable<string?> values) =>
        values
            .Select(NormalizeFileName)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(value => value!)
            .ToArray();
}
