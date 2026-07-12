using Marketplace.Api.Domain;
using Marketplace.Api.Features.Products.Admin.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Products.Admin.SaveSimilarProducts;

public static class SaveSimilarProductsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{id:int}/similar-products", async (
            int id,
            SimilarProductsRequest request,
            SaveSimilarProductsHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, request, cancellationToken));
    }
}

public sealed class SaveSimilarProductsHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(int id, SimilarProductsRequest request, CancellationToken cancellationToken)
    {
        await db.SimilarProducts.Where(item => item.ParentProductId == id).ExecuteDeleteAsync(cancellationToken);
        db.SimilarProducts.AddRange(request.ProductIds
            .Where(childId => childId != id)
            .Distinct()
            .Select(childId => new SimilarProduct
            {
                ParentProductId = id,
                ChildProductId = childId
            }));

        await db.SaveChangesAsync(cancellationToken);
        return Results.NoContent();
    }
}
