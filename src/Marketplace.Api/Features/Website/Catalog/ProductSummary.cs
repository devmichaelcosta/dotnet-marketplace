namespace Marketplace.Api.Features.Website.Catalog;

public sealed record ProductSummary(int Id, string Title, decimal Price, int Stock, bool Offer, string? Image, string Seller)
{
    public static ProductSummary From(Marketplace.Api.Domain.Product product) =>
        new(
            product.Id,
            product.Title,
            product.Price,
            product.Stock,
            product.Offer,
            ProductImagePath.Normalize(product.Images.FirstOrDefault()?.FileName),
            product.User?.Name ?? string.Empty);
}
