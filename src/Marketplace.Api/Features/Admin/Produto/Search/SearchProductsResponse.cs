using Marketplace.Api.Features.Admin.Produto.Shared;

namespace Marketplace.Api.Features.Admin.Produto.Search;

public sealed record SearchProductsResponse(
    AdminProductSummary[] Items,
    int Total,
    int Page,
    int PageSize);

