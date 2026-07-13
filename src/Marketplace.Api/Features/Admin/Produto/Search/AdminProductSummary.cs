namespace Marketplace.Api.Features.Admin.Produto.Search;

public sealed record AdminProductSummary(int Id, string Title, decimal Price, int Stock, bool Offer, string Sku, string Seller);
