using Marketplace.Api.Infrastructure.Security;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Modules.Produto.Website.GetLiked;

public sealed class GetLikedProductsHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(HttpContext http, CancellationToken cancellationToken)
    {
        var userId = http.User.GetUserId();
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var products = await db.ProductLikes
            .AsNoTracking()
            .Where(item => item.UserId == userId)
            .Include(item => item.Product)!.ThenInclude(product => product!.Images)
            .Include(item => item.Product)!.ThenInclude(product => product!.User)
            .OrderByDescending(item => item.CreatedAt)
            .Select(item => Marketplace.Api.Modules.Catalog.Website.Shared.ProductSummary.From(item.Product!))
            .ToListAsync(cancellationToken);

        return Results.Ok(products);
    }
}

