using Marketplace.Api.Modules.Catalog.Website.Shared;
namespace Marketplace.Api.Modules.Catalog.Website.GetById;

public sealed record CatalogProductDetailsResponse(ProductDetails Product, IReadOnlyList<ProductSummary> SimilarProducts);
