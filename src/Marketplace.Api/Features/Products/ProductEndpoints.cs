using Marketplace.Api.Domain;
using Marketplace.Api.Features;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Products;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var publicGroup = app.MapGroup("/api/products").WithTags("Products");

        publicGroup.MapPost("/{id:int}/like", async (int id, HttpContext http, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var userId = http.User.GetUserId();
            if (userId is null)
            {
                return Results.Unauthorized();
            }

            var exists = await db.ProductLikes.AnyAsync(item => item.ProductId == id && item.UserId == userId, cancellationToken);
            if (!exists)
            {
                db.ProductLikes.Add(new ProductLike { ProductId = id, UserId = userId.Value });
                await db.SaveChangesAsync(cancellationToken);
            }

            return Results.NoContent();
        }).RequireAuthorization();

        publicGroup.MapDelete("/{id:int}/like", async (int id, HttpContext http, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var userId = http.User.GetUserId();
            if (userId is null)
            {
                return Results.Unauthorized();
            }

            await db.ProductLikes.Where(item => item.ProductId == id && item.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            return Results.NoContent();
        }).RequireAuthorization();

        publicGroup.MapGet("/liked", async (HttpContext http, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var userId = http.User.GetUserId();
            if (userId is null)
            {
                return Results.Unauthorized();
            }

            var products = await db.ProductLikes
                .Where(item => item.UserId == userId)
                .Include(item => item.Product)!.ThenInclude(product => product!.Images)
                .Include(item => item.Product)!.ThenInclude(product => product!.User)
                .OrderByDescending(item => item.CreatedAt)
                .Select(item => Marketplace.Api.Features.Catalog.ProductSummary.From(item.Product!))
                .ToListAsync(cancellationToken);

            return Results.Ok(products);
        }).RequireAuthorization();

        publicGroup.MapPost("/{id:int}/ratings", async (int id, RatingRequest request, HttpContext http, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["rating"] = ["Titulo e descricao sao obrigatorios."] });
            }

            var userId = http.User.GetUserId();
            if (userId is null)
            {
                return Results.Unauthorized();
            }

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
            return Results.Accepted();
        }).RequireAuthorization();

        app.MapPost("/api/admin/ratings/{id:int}/approve", async (int id, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var rating = await db.ProductRatings.FindAsync([id], cancellationToken);
            if (rating is null)
            {
                return Results.NotFound();
            }

            rating.Approved = true;
            await db.SaveChangesAsync(cancellationToken);
            return Results.NoContent();
        }).RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole));

        app.MapGet("/api/admin/ratings/pending", async (
            string? search,
            string? sort,
            string? direction,
            MarketplaceDbContext db,
            CancellationToken cancellationToken) =>
        {
            var query = db.ProductRatings
                .Include(item => item.Product)
                .Include(item => item.User)
                .Where(item => !item.Approved);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(item =>
                    item.Product!.Title.Contains(search) ||
                    item.User!.Name.Contains(search) ||
                    item.Title.Contains(search) ||
                    item.Description.Contains(search));
            }

            query = (sort?.ToLowerInvariant(), direction?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true) switch
            {
                ("product", true) => query.OrderByDescending(item => item.Product!.Title),
                ("product", false) => query.OrderBy(item => item.Product!.Title),
                ("user", true) => query.OrderByDescending(item => item.User!.Name),
                ("user", false) => query.OrderBy(item => item.User!.Name),
                ("title", true) => query.OrderByDescending(item => item.Title),
                ("title", false) => query.OrderBy(item => item.Title),
                ("rating", true) => query.OrderByDescending(item => item.Rating),
                ("rating", false) => query.OrderBy(item => item.Rating),
                ("created", false) => query.OrderBy(item => item.CreatedAt),
                _ => query.OrderByDescending(item => item.CreatedAt)
            };

            return await query
                .Select(item => new PendingRatingResponse(
                    item.Id,
                    item.ProductId,
                    item.Product!.Title,
                    item.User!.Name,
                    item.Title,
                    item.Description,
                    item.Rating,
                    item.Recommended,
                    item.CreatedAt))
                .ToListAsync(cancellationToken);
        })
            .RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole));

        return app;
    }
}

public sealed record RatingRequest(string Title, string Description, bool Recommended, string Rating);
public sealed record PendingRatingResponse(
    int Id,
    int ProductId,
    string ProductTitle,
    string UserName,
    string Title,
    string Description,
    string Rating,
    bool Recommended,
    DateTimeOffset CreatedAt);
