namespace Marketplace.Api.Domain;

public sealed class SimilarProduct
{
    public int ParentProductId { get; set; }
    public Product? ParentProduct { get; set; }
    public int ChildProductId { get; set; }
    public Product? ChildProduct { get; set; }
}
