using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Website.Catalog.GetById;

public sealed class GetCatalogProductByIdHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(int id, CancellationToken cancellationToken)
    {
        var product = await db.Products
            .Include(item => item.Images)
            .Include(item => item.User)
            .Include(item => item.AttributeValues).ThenInclude(item => item.AttributeDefinition)
            .Include(item => item.Ratings.Where(rating => rating.Approved))
            .Include(item => item.SubCategory!).ThenInclude(item => item.Category)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (product is null)
        {
            return Results.NotFound();
        }

        var similar = await db.SimilarProducts
            .Where(item => item.ParentProductId == id)
            .Include(item => item.ChildProduct)!.ThenInclude(product => product!.Images)
            .Include(item => item.ChildProduct)!.ThenInclude(product => product!.User)
            .Select(item => ProductSummary.From(item.ChildProduct!))
            .ToListAsync(cancellationToken);

        return Results.Ok(new CatalogProductDetailsResponse(ProductDetails.From(product), similar));
    }
}
