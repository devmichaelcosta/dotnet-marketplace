using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Catalog;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/catalog").WithTags("Catalog");

        group.MapGet("/states", async (MarketplaceDbContext db, CancellationToken cancellationToken) =>
            await db.States
                .OrderBy(state => state.Name)
                .Select(state => new StateOption(state.Id, state.Name, state.Abbreviation))
                .ToListAsync(cancellationToken));

        group.MapGet("/home", async (MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var carousel = await db.CarouselImages.OrderBy(item => item.SortOrder).ToListAsync(cancellationToken);
            var categories = await db.Categories.OrderBy(item => item.Title).ToListAsync(cancellationToken);
            var offers = await db.Products
                .Include(product => product.Images)
                .Include(product => product.User)
                .Where(product => product.Offer && product.Stock > 0)
                .OrderBy(product => product.Title)
                .Take(12)
                .Select(product => ProductSummary.From(product))
                .ToListAsync(cancellationToken);

            return Results.Ok(new { Carousel = carousel, Categories = categories, Offers = offers });
        });

        group.MapGet("/products", async (
            string? search,
            int? categoryId,
            int? subCategoryId,
            int page,
            MarketplaceDbContext db,
            CancellationToken cancellationToken) =>
        {
            page = page <= 0 ? 1 : page;
            var query = db.Products.Include(product => product.Images).Include(product => product.User).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(product => product.Title.Contains(search) || product.Description.Contains(search) || product.Sku.Contains(search));
            }

            if (subCategoryId is not null)
            {
                query = query.Where(product => product.SubCategoryId == subCategoryId);
            }
            else if (categoryId is not null)
            {
                query = query.Where(product => product.SubCategory != null && product.SubCategory.CategoryId == categoryId);
            }

            var total = await query.CountAsync(cancellationToken);
            var items = await query.OrderBy(product => product.Title)
                .Skip((page - 1) * 12)
                .Take(12)
                .Select(product => ProductSummary.From(product))
                .ToListAsync(cancellationToken);

            return Results.Ok(new { Items = items, Total = total, Page = page, PageSize = 12 });
        });

        group.MapGet("/products/{id:int}", async (int id, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var product = await db.Products
                .Include(item => item.Images)
                .Include(item => item.User)
                .Include(item => item.AttributeValues).ThenInclude(item => item.AttributeDefinition)
                .Include(item => item.Ratings.Where(rating => rating.Approved))
                .Include(item => item.SubCategory!).ThenInclude(item => item.Category)
                .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

            if (product is null)
            {
                return Results.NotFound();
            }

            var similar = await db.SimilarProducts
                .Where(item => item.ParentProductId == id)
                .Include(item => item.ChildProduct)!.ThenInclude(product => product!.Images)
                .Include(item => item.ChildProduct)!.ThenInclude(product => product!.User)
                .Select(item => ProductSummary.From(item.ChildProduct!))
                .ToListAsync(cancellationToken);

            return Results.Ok(new { Product = ProductDetails.From(product), SimilarProducts = similar });
        });

        return app;
    }
}

public sealed record StateOption(int Id, string Name, string Abbreviation);

public sealed record ProductSummary(int Id, string Title, decimal Price, int Stock, bool Offer, string? Image, string Seller)
{
    public static ProductSummary From(Domain.Product product) =>
        new(product.Id, product.Title, product.Price, product.Stock, product.Offer, product.Images.FirstOrDefault()?.FileName, product.User?.Name ?? string.Empty);
}

public sealed record ProductDetails(
    int Id,
    string Title,
    string Description,
    decimal Price,
    int Stock,
    bool Offer,
    string Sku,
    string Seller,
    string? Category,
    string? SubCategory,
    string[] Images,
    ProductAttributeValue[] Attributes,
    ProductRatingResponse[] Ratings)
{
    public static ProductDetails From(Domain.Product product) =>
        new(
            product.Id,
            product.Title,
            product.Description,
            product.Price,
            product.Stock,
            product.Offer,
            product.Sku,
            product.User?.Name ?? string.Empty,
            product.SubCategory?.Category?.Title,
            product.SubCategory?.Title,
            product.Images.Select(image => image.FileName).ToArray(),
            product.AttributeValues.Select(value => new ProductAttributeValue(value.AttributeDefinition!.Name, value.Value)).ToArray(),
            product.Ratings.Where(rating => rating.Approved).Select(rating => new ProductRatingResponse(rating.Title, rating.Description, rating.Rating, rating.Recommended)).ToArray());
}

public sealed record ProductAttributeValue(string Attribute, string Value);
public sealed record ProductRatingResponse(string Title, string Description, string Rating, bool Recommended);
