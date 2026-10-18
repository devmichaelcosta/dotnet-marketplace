using System.Linq.Expressions;
using Marketplace.Api.Domain;

namespace Marketplace.Api.Modules.Catalog.Website.Shared;

public sealed record ProductSummary(int Id, string Title, decimal Price, int Stock, bool Offer, string? Image, string Seller)
{
    private static readonly Expression<Func<Product, ProductSummary>> SummaryProjection = product =>
        new ProductSummary(
            product.Id,
            product.Title,
            product.Price,
            product.Stock,
            product.Offer,
            ProductImagePath.Normalize(product.Images
                .OrderBy(image => image.Id)
                .Select(image => image.FileName)
                .FirstOrDefault()),
            product.User == null ? string.Empty : product.User.Name);

    private static readonly Func<Product, ProductSummary> SummaryFactory = SummaryProjection.Compile();

    public static Expression<Func<Product, ProductSummary>> Projection => SummaryProjection;

    public static ProductSummary From(Product product) => SummaryFactory(product);
}