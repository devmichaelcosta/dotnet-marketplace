using Marketplace.Api.Features.Admin.Produto.Shared;

namespace Marketplace.Api.Features.Admin.Produto.Create;

public sealed record CreateProductRequest(
    Guid? UserId,
    int? SubCategoryId,
    string Title,
    string Description,
    decimal Price,
    int Stock,
    bool Offer,
    string Sku,
    string[] Images,
    CreateProductAttributeRequest[] Attributes);

