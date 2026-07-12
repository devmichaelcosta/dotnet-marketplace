using Marketplace.Api.Domain;

namespace Marketplace.Api.Features.Products.Admin.Shared;


public sealed record ProductAttributeRequest(int AttributeId, string Value);

public sealed record SimilarProductsRequest(int[] ProductIds);

public sealed record AdminProductDetails(
    int Id,
    Guid UserId,
    int? SubCategoryId,
    string Title,
    string Description,
    decimal Price,
    int Stock,
    bool Offer,
    string Sku,
    string[] Images,
    ProductAttributeRequest[] Attributes,
    int[] SimilarProductIds)
{
    public static AdminProductDetails From(Product product, int[] similarProductIds) =>
        new(
            product.Id,
            product.UserId,
            product.SubCategoryId,
            product.Title,
            product.Description,
            product.Price,
            product.Stock,
            product.Offer,
            product.Sku,
            product.Images
                .Select(image => ProductImageStorage.NormalizeFileName(image.FileName))
                .Where(image => !string.IsNullOrWhiteSpace(image))
                .Select(image => image!)
                .ToArray(),
            product.AttributeValues.Select(value => new ProductAttributeRequest(value.AttributeDefinitionId, value.Value)).ToArray(),
            similarProductIds);
}
