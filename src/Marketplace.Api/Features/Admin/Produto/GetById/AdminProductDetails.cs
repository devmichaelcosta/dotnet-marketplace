using Marketplace.Api.Domain;
using Marketplace.Api.Features.Website.Produto.Shared;

namespace Marketplace.Api.Features.Admin.Produto.GetById;

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
    AdminProductAttributeResponse[] Attributes,
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
            product.AttributeValues
                .Select(value => new AdminProductAttributeResponse(value.AttributeDefinitionId, value.Value))
                .ToArray(),
            similarProductIds);
}


