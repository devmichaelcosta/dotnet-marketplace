namespace Marketplace.Api.Features.Website.Catalog;

public static class ProductImagePath
{
    public static string? Normalize(string? value)
    {
        var fileName = Marketplace.Api.Features.Website.Produto.Shared.ProductImageStorage.NormalizeFileName(value);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return null;
        }

        return $"/uploads/products/{fileName}";
    }
}
