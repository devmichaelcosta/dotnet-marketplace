using Marketplace.Api.Domain;
using Marketplace.Api.Features;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Website.Produto.Like;

public sealed class LikeProductHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(int id, HttpContext http, CancellationToken cancellationToken)
    {
        var userId = http.User.GetUserId();
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var exists = await db.ProductLikes
            .AnyAsync(item => item.ProductId == id && item.UserId == userId, cancellationToken);
        if (!exists)
        {
            await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
            db.ProductLikes.Add(new ProductLike { ProductId = id, UserId = userId.Value });
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        return Results.NoContent();
    }
}

