using Marketplace.Api.Modules.Produto.Admin.Shared;

namespace Marketplace.Api.Modules.Produto.Admin.Search;

public sealed record SearchProductsResponse(
    AdminProductSummary[] Items,
    int Total,
    int Page,
    int PageSize);

