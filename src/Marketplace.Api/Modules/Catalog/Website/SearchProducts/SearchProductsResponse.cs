using Marketplace.Api.Modules.Catalog.Website.Shared;
namespace Marketplace.Api.Modules.Catalog.Website.SearchProducts;

public sealed record SearchProductsResponse(IReadOnlyList<ProductSummary> Items, int Total, int Page, int PageSize);
