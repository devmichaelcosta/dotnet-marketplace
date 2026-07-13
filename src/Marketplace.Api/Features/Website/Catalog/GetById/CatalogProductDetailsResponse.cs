namespace Marketplace.Api.Features.Website.Catalog.GetById;

public sealed record CatalogProductDetailsResponse(ProductDetails Product, IReadOnlyList<ProductSummary> SimilarProducts);
