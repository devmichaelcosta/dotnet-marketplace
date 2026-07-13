using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Marketplace.Api.Features.Admin.Users.Delete;

public sealed class DeleteUserHandler(
    UserManager<ApplicationUser> userManager,
    MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return Results.NotFound();
        }

        var validation = await UserDeletionPolicy.ValidateAsync(db, id, cancellationToken);
        if (validation is not null)
        {
            return validation;
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return Results.ValidationProblem(result.Errors.ToDictionary(error => error.Code, error => new[] { error.Description }));
        }

        await transaction.CommitAsync(cancellationToken);
        return Results.NoContent();
    }
}
