using Marketplace.Api.Domain;
using Marketplace.Api.Modules.Catalog.Website.Shared;

namespace Marketplace.Tests;

public sealed class ProductSummaryProjectionTests
{
    [Fact]
    public void Projection_returns_normalized_first_image_and_seller_for_product_cards()
    {
        var product = new Product
        {
            Id = 42,
            Title = "Produto com imagem",
            Price = 19.99m,
            Stock = 3,
            Offer = true,
            User = new ApplicationUser { Name = "Vendedor" },
            Images =
            [
                new ProductImage { Id = 2, FileName = "uploads/products/second.jpg" },
                new ProductImage { Id = 1, FileName = "uploads/products/first.jpg" }
            ]
        };

        var summary = ProductSummary.From(product);

        Assert.Equal("/uploads/products/first.jpg", summary.Image);
        Assert.Equal("Vendedor", summary.Seller);
        Assert.True(summary.Offer);
    }
}