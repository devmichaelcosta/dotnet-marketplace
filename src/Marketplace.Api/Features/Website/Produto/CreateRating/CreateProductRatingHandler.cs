using Marketplace.Api.Domain;
using Marketplace.Api.Features;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Website.Produto.CreateRating;

public sealed class CreateProductRatingHandler(MarketplaceDbContext db)
{
    public async Task<IResult> HandleAsync(
        int id,
        CreateProductRatingRequest request,
        HttpContext http,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["rating"] = ["Titulo e descricao sao obrigatorios."]
            });
        }

        var userId = http.User.GetUserId();
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        db.ProductRatings.Add(new ProductRating
        {
            ProductId = id,
            UserId = userId.Value,
            Title = request.Title,
            Description = request.Description,
            Recommended = request.Recommended,
            Rating = request.Rating,
            Approved = false
        });
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Results.Accepted();
    }
}

