using Marketplace.Api.Domain;
using Marketplace.Api.Features.Website.Users.Shared;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Website.Users.CreateAddress;

public sealed class CreateAddressHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(AddressRequest request, HttpContext http, CancellationToken cancellationToken)
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

        var address = new Address
        {
            UserId = userId.Value,
            StateId = request.StateId,
            Street = request.Street.Trim(),
            Cep = request.Cep.Trim(),
            Neighborhood = request.Neighborhood.Trim(),
            City = request.City.Trim(),
            Complement = request.Complement
        };

        db.Addresses.Add(address);
        await db.SaveChangesAsync(cancellationToken);
        await db.Entry(address).Reference(item => item.State).LoadAsync(cancellationToken);

        return Results.Created($"/api/users/me/addresses/{address.Id}", AddressResponse.From(address));
    }
}
