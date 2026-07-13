namespace Marketplace.Api.Features.Admin.Produto.ProductImports.Shared;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);

