using Marketplace.Api.Modules.Produto.Admin.Shared;

namespace Marketplace.Api.Modules.Produto.Admin.Create;

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

