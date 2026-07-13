using Marketplace.Api.Features.Admin.Sellers.Search;
using Marketplace.Api.Features.Admin.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Sellers.Update;

public sealed class UpdateSellerHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(Guid id, UpdateSellerRequest request, CancellationToken cancellationToken)
    {
        var validationErrors = AdminValidationPolicy.ValidateSeller(request);
        if (validationErrors.Count > 0)
        {
            return Results.ValidationProblem(validationErrors);
        }

        var seller = await db.Sellers.Include(item => item.User).FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (seller is null)
        {
            return Results.NotFound();
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        seller.Email = request.Email;
        seller.Website = request.Website;
        seller.Company = request.Company;
        seller.Cnpj = request.Cnpj;
        seller.BranchOfActivity = request.BranchOfActivity;
        seller.FantasyName = request.FantasyName;
        seller.Age = request.Age;
        seller.DateOfBirth = request.DateOfBirth;
        if (seller.User is not null)
        {
            seller.User.Name = request.Name;
            seller.User.LastName = request.LastName;
            seller.User.Cpf = AdminValidationPolicy.NormalizeDocument(request.Cpf);
        }

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Results.Ok(SellerResponse.From(seller));
    }
}
