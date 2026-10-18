namespace Marketplace.Api.Modules.Produto.Admin.Update;

public sealed record UpdateProductRequest(
    Guid? UserId,
    int? SubCategoryId,
    string Title,
    string Description,
    decimal Price,
    int Stock,
    bool Offer,
    string Sku,
    string[] Images,
    UpdateProductAttributeRequest[] Attributes);

