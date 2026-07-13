using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Website.Users.DeleteAddress;

public sealed class DeleteAddressHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(Guid id, HttpContext http, CancellationToken cancellationToken)
    {
        var userId = http.User.GetUserId();
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var deleted = await db.Addresses
            .Where(item => item.Id == id && item.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

        return deleted == 0 ? Results.NotFound() : Results.NoContent();
    }
}
