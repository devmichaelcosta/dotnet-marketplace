using Marketplace.Api.Infrastructure.Security;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Modules.Produto.Website.Unlike;

public sealed class UnlikeProductHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(int id, HttpContext http, CancellationToken cancellationToken)
    {
        var userId = http.User.GetUserId();
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        await db.ProductLikes
            .Where(item => item.ProductId == id && item.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Results.NoContent();
    }
}

