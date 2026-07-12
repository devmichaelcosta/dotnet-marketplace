using Marketplace.Api.Features.Products.Admin.Shared;

namespace Marketplace.Api.Features.Products.Admin.Create;

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
    ProductAttributeRequest[] Attributes);
