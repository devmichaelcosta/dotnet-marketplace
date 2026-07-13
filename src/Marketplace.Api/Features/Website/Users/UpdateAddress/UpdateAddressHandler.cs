using Marketplace.Api.Features.Website.Users.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Website.Users.UpdateAddress;

public sealed class UpdateAddressHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(Guid id, AddressRequest request, HttpContext http, CancellationToken cancellationToken)
    {
        var userId = http.User.GetUserId();
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var validation = AddressValidationPolicy.Validate(request);
        if (validation is not null)
        {
            return Results.ValidationProblem(validation);
        }

        var address = await db.Addresses.FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId, cancellationToken);
        if (address is null)
        {
            return Results.NotFound();
        }

        address.StateId = request.StateId;
        address.Street = request.Street.Trim();
        address.Cep = request.Cep.Trim();
        address.Neighborhood = request.Neighborhood.Trim();
        address.City = request.City.Trim();
        address.Complement = request.Complement;
        await db.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
