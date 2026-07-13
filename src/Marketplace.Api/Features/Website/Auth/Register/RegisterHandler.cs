using Marketplace.Api.Domain;
using Marketplace.Api.Features.Website.Auth.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace Marketplace.Api.Features.Website.Auth.Register;

public sealed class RegisterHandler(
    UserManager<ApplicationUser> userManager,
    MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var validation = RegistrationValidation.Validate(request);
        if (validation is not null)
        {
            return Results.ValidationProblem(validation);
        }

        await using IDbContextTransaction transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        var user = new ApplicationUser
        {
            UserName = request.Login.Trim(),
            Email = $"{request.Login.Trim()}@marketplace.local",
            Name = request.Name.Trim(),
            LastName = request.LastName.Trim(),
            Cpf = DocumentNormalizer.Normalize(request.Cpf),
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return Results.ValidationProblem(result.Errors.ToDictionary(error => error.Code, error => new[] { error.Description }));
        }

        var roleResult = await userManager.AddToRoleAsync(user, MarketplaceSeed.CustomerRole);
        if (!roleResult.Succeeded)
        {
            return Results.ValidationProblem(roleResult.Errors.ToDictionary(error => error.Code, error => new[] { error.Description }));
        }

        await transaction.CommitAsync(cancellationToken);
        return Results.Created($"/api/users/{user.Id}", new { user.Id, user.UserName, user.Name });
    }
}
