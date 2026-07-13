using Marketplace.Api.Features.Admin.Produto.Create;

namespace Marketplace.Tests;

public sealed class ProductPolicyTests
{
    [Fact]
    public void Product_policy_accepts_valid_product_request()
    {
        var request = ValidRequest();
        var validator = new CreateProductRequestValidator();

        var errors = validator.Validate(request);

        Assert.Empty(errors);
    }

    [Fact]
    public void Product_policy_requires_core_product_fields()
    {
        var request = ValidRequest() with
        {
            Title = " ",
            Description = "",
            Sku = " ",
            Price = 0,
            Stock = -1
        };
        var validator = new CreateProductRequestValidator();

        var errors = validator.Validate(request);

        Assert.Contains("title", errors.Keys);
        Assert.Contains("description", errors.Keys);
        Assert.Contains("sku", errors.Keys);
        Assert.Contains("price", errors.Keys);
        Assert.Contains("stock", errors.Keys);
    }

    private static CreateProductRequest ValidRequest() =>
        new(
            UserId: null,
            SubCategoryId: 1,
            Title: "Produto",
            Description: "Descricao",
            Price: 10m,
            Stock: 5,
            Offer: false,
            Sku: "SKU-1",
            Images: [],
            Attributes: []);
}

