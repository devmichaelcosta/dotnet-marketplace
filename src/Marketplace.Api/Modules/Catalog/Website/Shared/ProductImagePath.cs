namespace Marketplace.Api.Modules.Catalog.Website.Shared;

public static class ProductImagePath
{
    public static string? Normalize(string? value)
    {
        var fileName = Marketplace.Api.Modules.Produto.Website.Shared.ProductImageStorage.NormalizeFileName(value);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return null;
        }

        return $"/uploads/products/{fileName}";
    }
}
