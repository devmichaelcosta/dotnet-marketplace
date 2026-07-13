using Marketplace.Api.Features.Website.Produto.Shared;
using Marketplace.Api.Domain;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.Produto.Shared;

public sealed class ProductImagesWriter
{
    public List<ProductImage> Build(IEnumerable<string?> images, int? productId = null) =>
        ProductImageStorage.NormalizeFileNames(images)
            .Select(image => new ProductImage
            {
                ProductId = productId ?? 0,
                FileName = image
            })
            .ToList();

    public void Replace(Product product, IEnumerable<string?> images, MarketplaceDbContext db)
    {
        db.ProductImages.RemoveRange(product.Images);
        product.Images = Build(images, product.Id);
    }
}



