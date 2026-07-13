using Marketplace.Api.Domain;
using Marketplace.Api.Features.Website.Users.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Marketplace.Api.Features.Website.Users.UpdateProfile;

public sealed class UpdateProfileHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(ProfileRequest request, HttpContext http, CancellationToken cancellationToken)
    {
        var userId = http.User.GetUserId();
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var validation = ProfilePolicy.Validate(request);
        if (validation is not null)
        {
            return Results.ValidationProblem(validation);
        }

        var user = await db.Users
            .Include(item => item.Addresses)
            .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound();
        }

        await using IDbContextTransaction transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        user.Name = request.Name.Trim();
        user.LastName = request.LastName.Trim();
        user.Cpf = ProfilePolicy.NormalizeDocument(request.Cpf);

        db.Addresses.RemoveRange(user.Addresses);
        user.Addresses = request.Addresses.Select(address => new Address
        {
            UserId = user.Id,
            StateId = address.StateId,
            Street = address.Street.Trim(),
            Cep = address.Cep.Trim(),
            Neighborhood = address.Neighborhood.Trim(),
            City = address.City.Trim(),
            Complement = address.Complement
        }).ToList();

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Results.NoContent();
    }
}
