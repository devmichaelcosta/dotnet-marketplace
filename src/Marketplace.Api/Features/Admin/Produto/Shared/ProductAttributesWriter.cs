using Marketplace.Api.Domain;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.Produto.Shared;

public sealed class ProductAttributesWriter
{
    public List<ProductAttributeValue> Build(IEnumerable<(int AttributeId, string Value)> attributes, int? productId = null) =>
        attributes
            .Select(value => new ProductAttributeValue
            {
                ProductId = productId ?? 0,
                AttributeDefinitionId = value.AttributeId,
                Value = value.Value
            })
            .ToList();

    public void Replace(Product product, IEnumerable<(int AttributeId, string Value)> attributes, MarketplaceDbContext db)
    {
        db.ProductAttributeValues.RemoveRange(product.AttributeValues);
        product.AttributeValues = Build(attributes, product.Id);
    }
}

