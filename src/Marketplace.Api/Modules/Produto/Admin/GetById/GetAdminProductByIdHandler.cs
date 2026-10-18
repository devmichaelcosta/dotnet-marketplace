using Marketplace.Api.Modules.Produto.Admin.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Modules.Produto.Admin.GetById;

public sealed class GetAdminProductByIdHandler(MarketplaceDbContext db, ProductAdminAccessPolicy accessPolicy)
{
    public async Task<IResult> HandleAsync(int id, HttpContext http, CancellationToken cancellationToken)
    {
        var product = await db.Products
            .Include(item => item.Images)
            .Include(item => item.AttributeValues)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (product is null)
        {
            return Results.NotFound();
        }

        var actor = accessPolicy.ResolveActor(http);
        if (!accessPolicy.CanManage(product, actor))
        {
            return Results.Forbid();
        }

        var similarProductIds = await db.SimilarProducts
            .Where(item => item.ParentProductId == id)
            .Select(item => item.ChildProductId)
            .ToArrayAsync(cancellationToken);

        return Results.Ok(AdminProductDetails.From(product, similarProductIds));
    }
}

