using Marketplace.Api.Domain;
using Marketplace.Api.Features;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Users;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").RequireAuthorization().WithTags("Users");

        group.MapGet("/me", async (HttpContext http, UserManager<ApplicationUser> userManager, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var userId = http.User.GetUserId();
            if (userId is null)
            {
                return Results.Unauthorized();
            }

            var user = await db.Users
                .Include(item => item.Addresses).ThenInclude(item => item.State)
                .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken);

            if (user is null)
            {
                return Results.NotFound();
            }

            var roles = await userManager.GetRolesAsync(user);
            return Results.Ok(ProfileResponse.From(user, roles.ToArray()));
        });

        group.MapPut("/me", async (ProfileRequest request, HttpContext http, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var userId = http.User.GetUserId();
            if (userId is null)
            {
                return Results.Unauthorized();
            }

            var user = await db.Users
                .Include(item => item.Addresses)
                .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken);

            if (user is null)
            {
                return Results.NotFound();
            }

            user.Name = request.Name.Trim();
            user.LastName = request.LastName.Trim();
            user.Cpf = request.Cpf;

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
            return Results.NoContent();
        });

        group.MapPost("/me/addresses", async (AddressRequest request, HttpContext http, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var userId = http.User.GetUserId();
            if (userId is null)
            {
                return Results.Unauthorized();
            }

            var validation = ValidateAddress(request);
            if (validation is not null)
            {
                return validation;
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
        });

        group.MapPut("/me/addresses/{id:guid}", async (Guid id, AddressRequest request, HttpContext http, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var userId = http.User.GetUserId();
            if (userId is null)
            {
                return Results.Unauthorized();
            }

            var validation = ValidateAddress(request);
            if (validation is not null)
            {
                return validation;
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
        });

        group.MapDelete("/me/addresses/{id:guid}", async (Guid id, HttpContext http, MarketplaceDbContext db, CancellationToken cancellationToken) =>
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
        });

        return app;
    }

    private static IResult? ValidateAddress(AddressRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        if (request.StateId <= 0)
        {
            errors["stateId"] = ["Estado obrigatorio."];
        }

        if (string.IsNullOrWhiteSpace(request.Street))
        {
            errors["street"] = ["Rua obrigatoria."];
        }

        if (string.IsNullOrWhiteSpace(request.Cep))
        {
            errors["cep"] = ["CEP obrigatorio."];
        }

        if (string.IsNullOrWhiteSpace(request.Neighborhood))
        {
            errors["neighborhood"] = ["Bairro obrigatorio."];
        }

        if (string.IsNullOrWhiteSpace(request.City))
        {
            errors["city"] = ["Cidade obrigatoria."];
        }

        return errors.Count == 0 ? null : Results.ValidationProblem(errors);
    }
}

public sealed record ProfileResponse(Guid Id, string Login, string Name, string LastName, string? Cpf, string[] Roles, AddressResponse[] Addresses)
{
    public static ProfileResponse From(ApplicationUser user, string[] roles) =>
        new(
            user.Id,
            user.UserName ?? string.Empty,
            user.Name,
            user.LastName,
            user.Cpf,
            roles,
            user.Addresses.Select(AddressResponse.From).ToArray());
}

public sealed record AddressResponse(Guid Id, int StateId, string State, string Street, string Cep, string Neighborhood, string City, string? Complement)
{
    public static AddressResponse From(Address address) =>
        new(address.Id, address.StateId, address.State?.Abbreviation ?? string.Empty, address.Street, address.Cep, address.Neighborhood, address.City, address.Complement);
}

public sealed record ProfileRequest(string Name, string LastName, string? Cpf, AddressRequest[] Addresses);
public sealed record AddressRequest(int StateId, string Street, string Cep, string Neighborhood, string City, string? Complement);
