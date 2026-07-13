using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Sellers.Search;
using Marketplace.Api.Features.Admin.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Marketplace.Api.Features.Admin.Sellers.Create;

public sealed class CreateSellerHandler(
    UserManager<ApplicationUser> userManager,
    MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(CreateSellerRequest request, CancellationToken cancellationToken)
    {
        var validationErrors = AdminValidationPolicy.ValidateSeller(request, passwordRequired: true);
        if (validationErrors.Count > 0)
        {
            return Results.ValidationProblem(validationErrors);
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var user = new ApplicationUser
        {
            UserName = request.Login.Trim(),
            Email = $"{request.Login.Trim()}@marketplace.local",
            Name = request.Name.Trim(),
            LastName = request.LastName.Trim(),
            Cpf = AdminValidationPolicy.NormalizeDocument(request.Cpf),
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, request.Password!);
        if (!result.Succeeded)
        {
            return Results.ValidationProblem(result.Errors.ToDictionary(error => error.Code, error => new[] { error.Description }));
        }

        await userManager.AddToRoleAsync(user, MarketplaceSeed.SellerRole);
        var seller = new Seller
        {
            UserId = user.Id,
            Age = request.Age,
            Email = request.Email,
            DateOfBirth = request.DateOfBirth,
            Website = request.Website,
            Company = request.Company,
            Cnpj = request.Cnpj,
            BranchOfActivity = request.BranchOfActivity,
            FantasyName = request.FantasyName
        };

        db.Sellers.Add(seller);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        seller.User = user;
        return Results.Created($"/api/admin/sellers/{seller.Id}", SellerResponse.From(seller));
    }
}
