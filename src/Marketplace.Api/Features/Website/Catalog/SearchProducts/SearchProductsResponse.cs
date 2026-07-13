namespace Marketplace.Api.Features.Website.Catalog.SearchProducts;

public sealed record SearchProductsResponse(IReadOnlyList<ProductSummary> Items, int Total, int Page, int PageSize);
