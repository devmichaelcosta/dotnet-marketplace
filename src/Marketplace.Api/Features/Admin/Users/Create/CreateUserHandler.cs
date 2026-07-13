using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Shared;
using Marketplace.Api.Features.Admin.Users.Search;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Marketplace.Api.Features.Admin.Users.Create;

public sealed class CreateUserHandler(
    UserManager<ApplicationUser> userManager,
    MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var validationErrors = AdminValidationPolicy.ValidateUser(request, passwordRequired: true);
        if (validationErrors.Count > 0)
        {
            return Results.ValidationProblem(validationErrors);
        }

        var user = new ApplicationUser
        {
            UserName = request.Login.Trim(),
            Email = $"{request.Login.Trim()}@marketplace.local",
            Name = request.Name.Trim(),
            LastName = request.LastName.Trim(),
            Cpf = AdminValidationPolicy.NormalizeDocument(request.Cpf),
            EmailConfirmed = true
        };

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var result = await userManager.CreateAsync(user, request.Password!);
        if (!result.Succeeded)
        {
            return Results.ValidationProblem(result.Errors.ToDictionary(error => error.Code, error => new[] { error.Description }));
        }

        var role = UserRoleNormalizer.NormalizeRole(request.Role);
        await userManager.AddToRoleAsync(user, role);
        await transaction.CommitAsync(cancellationToken);
        return Results.Created($"/api/admin/users/{user.Id}", UserResponse.From(user, [role]));
    }
}
