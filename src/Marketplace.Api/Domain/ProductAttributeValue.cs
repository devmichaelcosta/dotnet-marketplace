namespace Marketplace.Api.Domain;

public sealed class ProductAttributeValue
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int AttributeDefinitionId { get; set; }
    public AttributeDefinition? AttributeDefinition { get; set; }
    public string Value { get; set; } = string.Empty;
}
