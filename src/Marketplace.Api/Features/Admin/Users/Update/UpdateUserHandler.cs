using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Shared;
using Marketplace.Api.Features.Admin.Users.Search;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Marketplace.Api.Features.Admin.Users.Update;

public sealed class UpdateUserHandler(
    UserManager<ApplicationUser> userManager,
    MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var validationErrors = AdminValidationPolicy.ValidateUser(request, passwordRequired: false);
        if (validationErrors.Count > 0)
        {
            return Results.ValidationProblem(validationErrors);
        }

        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return Results.NotFound();
        }

        user.UserName = request.Login.Trim();
        user.Email = $"{request.Login.Trim()}@marketplace.local";
        user.Name = request.Name.Trim();
        user.LastName = request.LastName.Trim();
        user.Cpf = AdminValidationPolicy.NormalizeDocument(request.Cpf);

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return Results.ValidationProblem(updateResult.Errors.ToDictionary(error => error.Code, error => new[] { error.Description }));
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        if (currentRoles.Count > 0)
        {
            await userManager.RemoveFromRolesAsync(user, currentRoles);
        }

        var role = UserRoleNormalizer.NormalizeRole(request.Role);
        await userManager.AddToRoleAsync(user, role);
        await transaction.CommitAsync(cancellationToken);
        return Results.Ok(UserResponse.From(user, [role]));
    }
}
