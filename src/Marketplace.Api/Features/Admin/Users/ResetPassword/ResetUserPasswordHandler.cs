using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Marketplace.Api.Features.Admin.Users.ResetPassword;

public sealed class ResetUserPasswordHandler(
    UserManager<ApplicationUser> userManager,
    MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(Guid id, ResetUserPasswordRequest request, CancellationToken cancellationToken)
    {
        var validationErrors = AdminPasswordResetPolicy.Validate(request);
        if (validationErrors.Count > 0)
        {
            return Results.ValidationProblem(validationErrors);
        }

        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return Results.NotFound();
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var passwordResult = await userManager.ResetPasswordAsync(user, token, request.Password);
        if (!passwordResult.Succeeded)
        {
            return Results.ValidationProblem(passwordResult.Errors.ToDictionary(error => error.Code, error => new[] { error.Description }));
        }

        await transaction.CommitAsync(cancellationToken);
        return Results.NoContent();
    }
}
