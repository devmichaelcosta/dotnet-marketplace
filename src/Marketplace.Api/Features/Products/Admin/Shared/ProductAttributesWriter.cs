using Marketplace.Api.Domain;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Products.Admin.Shared;

public sealed class ProductAttributesWriter
{
    public List<ProductAttributeValue> Build(IEnumerable<ProductAttributeRequest> attributes, int? productId = null) =>
        attributes
            .Select(value => new ProductAttributeValue
            {
                ProductId = productId ?? 0,
                AttributeDefinitionId = value.AttributeId,
                Value = value.Value
            })
            .ToList();

    public void Replace(Product product, IEnumerable<ProductAttributeRequest> attributes, MarketplaceDbContext db)
    {
        db.ProductAttributeValues.RemoveRange(product.AttributeValues);
        product.AttributeValues = Build(attributes, product.Id);
    }
}
