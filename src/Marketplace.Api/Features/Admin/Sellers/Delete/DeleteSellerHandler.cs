using Marketplace.Api.Features.Admin.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Sellers.Delete;

public sealed class DeleteSellerHandler(
    UserManager<Marketplace.Api.Domain.ApplicationUser> userManager,
    MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var seller = await db.Sellers.Include(item => item.User).FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (seller is null)
        {
            return Results.NotFound();
        }

        var validation = await SellerDeletionPolicy.ValidateAsync(db, seller.UserId, cancellationToken);
        if (validation is not null)
        {
            return validation;
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        db.Sellers.Remove(seller);
        await db.SaveChangesAsync(cancellationToken);

        if (seller.User is not null)
        {
            await userManager.RemoveFromRoleAsync(seller.User, MarketplaceSeed.SellerRole);
            if (!await userManager.IsInRoleAsync(seller.User, MarketplaceSeed.CustomerRole))
            {
                await userManager.AddToRoleAsync(seller.User, MarketplaceSeed.CustomerRole);
            }
        }

        await transaction.CommitAsync(cancellationToken);
        return Results.NoContent();
    }
}
