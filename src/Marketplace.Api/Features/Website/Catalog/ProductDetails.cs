namespace Marketplace.Api.Features.Website.Catalog;

public sealed record ProductDetails(
    int Id,
    string Title,
    string Description,
    decimal Price,
    int Stock,
    bool Offer,
    string Sku,
    string Seller,
    string? Category,
    string? SubCategory,
    string[] Images,
    ProductAttributeValue[] Attributes,
    ProductRatingResponse[] Ratings)
{
    public static ProductDetails From(Marketplace.Api.Domain.Product product) =>
        new(
            product.Id,
            product.Title,
            product.Description,
            product.Price,
            product.Stock,
            product.Offer,
            product.Sku,
            product.User?.Name ?? string.Empty,
            product.SubCategory?.Category?.Title,
            product.SubCategory?.Title,
            product.Images
                .Select(image => ProductImagePath.Normalize(image.FileName))
                .Where(image => !string.IsNullOrWhiteSpace(image))
                .Select(image => image!)
                .ToArray(),
            product.AttributeValues.Select(value => new ProductAttributeValue(value.AttributeDefinition!.Name, value.Value)).ToArray(),
            product.Ratings.Where(rating => rating.Approved).Select(rating => new ProductRatingResponse(rating.Title, rating.Description, rating.Rating, rating.Recommended)).ToArray());
}
