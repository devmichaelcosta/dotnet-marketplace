using Marketplace.Api.Features.Products.Admin.Shared;

namespace Marketplace.Api.Features.Products.Admin.Search;

public sealed record SearchProductsResponse(
    AdminProductSummary[] Items,
    int Total,
    int Page,
    int PageSize);
