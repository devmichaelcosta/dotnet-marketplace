using Marketplace.Api.Domain;
using Marketplace.Api.Features;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Products;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/products").RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole, MarketplaceSeed.SellerRole));

        group.MapGet("/", async (string? search, int page, int pageSize, string? sort, string? direction, HttpContext http, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : Math.Clamp(pageSize, 1, 100);
            direction = string.Equals(direction, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";
            var userId = http.User.GetUserId();
            var isSeller = http.User.IsInRole(MarketplaceSeed.SellerRole) && !http.User.IsInRole(MarketplaceSeed.AdminRole);
            var query = db.Products.Include(item => item.Images).Include(item => item.User).AsQueryable();
            if (isSeller)
            {
                query = query.Where(item => item.UserId == userId);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(item => item.Title.Contains(search) || item.Description.Contains(search) || item.Sku.Contains(search));
            }

            var total = await query.CountAsync(cancellationToken);
            query = (sort?.ToLowerInvariant(), direction) switch
            {
                ("sku", "desc") => query.OrderByDescending(item => item.Sku),
                ("sku", _) => query.OrderBy(item => item.Sku),
                ("stock", "desc") => query.OrderByDescending(item => item.Stock),
                ("stock", _) => query.OrderBy(item => item.Stock),
                ("price", "desc") => query.OrderByDescending(item => item.Price),
                ("price", _) => query.OrderBy(item => item.Price),
                ("seller", "desc") => query.OrderByDescending(item => item.User!.Name),
                ("seller", _) => query.OrderBy(item => item.User!.Name),
                ("offer", "desc") => query.OrderByDescending(item => item.Offer),
                ("offer", _) => query.OrderBy(item => item.Offer),
                _ when direction == "desc" => query.OrderByDescending(item => item.Title),
                _ => query.OrderBy(item => item.Title)
            };

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(item => new AdminProductSummary(item.Id, item.Title, item.Price, item.Stock, item.Offer, item.Sku, item.User!.Name))
                .ToListAsync(cancellationToken);
            return Results.Ok(new { Items = items, Total = total, Page = page, PageSize = pageSize });
        });

        group.MapGet("/{id:int}", async (int id, HttpContext http, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var product = await db.Products
                .Include(item => item.Images)
                .Include(item => item.AttributeValues)
                .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

            if (product is null)
            {
                return Results.NotFound();
            }

            if (http.User.IsInRole(MarketplaceSeed.SellerRole) && !http.User.IsInRole(MarketplaceSeed.AdminRole) && product.UserId != http.User.GetUserId())
            {
                return Results.Forbid();
            }

            var similarProductIds = await db.SimilarProducts
                .Where(item => item.ParentProductId == id)
                .Select(item => item.ChildProductId)
                .ToArrayAsync(cancellationToken);

            return Results.Ok(AdminProductDetails.From(product, similarProductIds));
        });

        group.MapPost("/", async (ProductRequest request, HttpContext http, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var errors = ProductPolicy.Validate(request);
            if (errors.Count > 0)
            {
                return Results.ValidationProblem(errors);
            }

            var ownerId = http.User.IsInRole(MarketplaceSeed.AdminRole) && request.UserId is not null
                ? request.UserId.Value
                : http.User.GetUserId();
            if (ownerId is null)
            {
                return Results.Unauthorized();
            }

            var product = new Product
            {
                UserId = ownerId.Value,
                SubCategoryId = request.SubCategoryId,
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                Price = request.Price,
                Stock = request.Stock,
                Offer = request.Offer,
                Sku = request.Sku.Trim(),
                CreatedBy = http.User.Identity?.Name ?? "system",
                Images = request.Images.Select(image => new ProductImage { FileName = image }).ToList(),
                AttributeValues = request.Attributes.Select(value => new ProductAttributeValue
                {
                    AttributeDefinitionId = value.AttributeId,
                    Value = value.Value
                }).ToList()
            };

            db.Products.Add(product);
            await db.SaveChangesAsync(cancellationToken);
            return Results.Created($"/api/admin/products/{product.Id}", new { product.Id });
        });

        group.MapPut("/{id:int}", async (int id, ProductRequest request, HttpContext http, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var errors = ProductPolicy.Validate(request);
            if (errors.Count > 0)
            {
                return Results.ValidationProblem(errors);
            }

            var product = await db.Products.Include(item => item.Images).Include(item => item.AttributeValues).FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
            if (product is null)
            {
                return Results.NotFound();
            }

            if (http.User.IsInRole(MarketplaceSeed.SellerRole) && !http.User.IsInRole(MarketplaceSeed.AdminRole) && product.UserId != http.User.GetUserId())
            {
                return Results.Forbid();
            }

            product.SubCategoryId = request.SubCategoryId;
            product.Title = request.Title.Trim();
            product.Description = request.Description.Trim();
            product.Price = request.Price;
            product.Stock = request.Stock;
            product.Offer = request.Offer;
            product.Sku = request.Sku.Trim();

            db.ProductImages.RemoveRange(product.Images);
            db.ProductAttributeValues.RemoveRange(product.AttributeValues);
            product.Images = request.Images.Select(image => new ProductImage { ProductId = product.Id, FileName = image }).ToList();
            product.AttributeValues = request.Attributes.Select(value => new ProductAttributeValue
            {
                ProductId = product.Id,
                AttributeDefinitionId = value.AttributeId,
                Value = value.Value
            }).ToList();

            await db.SaveChangesAsync(cancellationToken);
            return Results.Ok(new { product.Id });
        });

        group.MapDelete("/{id:int}", async (int id, HttpContext http, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            var product = await db.Products.FindAsync([id], cancellationToken);
            if (product is null)
            {
                return Results.NotFound();
            }

            if (http.User.IsInRole(MarketplaceSeed.SellerRole) && !http.User.IsInRole(MarketplaceSeed.AdminRole) && product.UserId != http.User.GetUserId())
            {
                return Results.Forbid();
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync(cancellationToken);
            return Results.NoContent();
        });

        group.MapDelete("/{id:int}/images/{fileName}", async (int id, string fileName, HttpContext http, MarketplaceDbContext db, IWebHostEnvironment environment, CancellationToken cancellationToken) =>
        {
            var product = await db.Products.Include(item => item.Images).FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
            if (product is null)
            {
                return Results.NotFound();
            }

            if (http.User.IsInRole(MarketplaceSeed.SellerRole) && !http.User.IsInRole(MarketplaceSeed.AdminRole) && product.UserId != http.User.GetUserId())
            {
                return Results.Forbid();
            }

            var sanitized = Path.GetFileName(fileName);
            var image = product.Images.FirstOrDefault(item => item.FileName == sanitized);
            if (image is null)
            {
                return Results.NotFound();
            }

            db.ProductImages.Remove(image);
            await db.SaveChangesAsync(cancellationToken);

            var basePath = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
            var absolutePath = Path.Combine(basePath, "uploads", "products", sanitized);
            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
            }

            return Results.NoContent();
        });

        group.MapPost("/{id:int}/similar-products", async (int id, SimilarProductsRequest request, MarketplaceDbContext db, CancellationToken cancellationToken) =>
        {
            await db.SimilarProducts.Where(item => item.ParentProductId == id).ExecuteDeleteAsync(cancellationToken);
            db.SimilarProducts.AddRange(request.ProductIds.Where(childId => childId != id).Distinct().Select(childId => new SimilarProduct
            {
                ParentProductId = id,
                ChildProductId = childId
            }));
            await db.SaveChangesAsync(cancellationToken);
            return Results.NoContent();
        });

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

public static class ProductPolicy
{
    public static Dictionary<string, string[]> Validate(ProductRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            errors["title"] = ["Titulo obrigatorio."];
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            errors["description"] = ["Descricao obrigatoria."];
        }

        if (string.IsNullOrWhiteSpace(request.Sku))
        {
            errors["sku"] = ["SKU obrigatorio."];
        }

        if (request.Price <= 0)
        {
            errors["price"] = ["Preco deve ser maior que zero."];
        }

        if (request.Stock < 0)
        {
            errors["stock"] = ["Estoque nao pode ser negativo."];
        }

        return errors;
    }
}

public sealed record ProductRequest(
    Guid? UserId,
    int? SubCategoryId,
    string Title,
    string Description,
    decimal Price,
    int Stock,
    bool Offer,
    string Sku,
    string[] Images,
    ProductAttributeRequest[] Attributes);

public sealed record ProductAttributeRequest(int AttributeId, string Value);
public sealed record SimilarProductsRequest(int[] ProductIds);
public sealed record RatingRequest(string Title, string Description, bool Recommended, string Rating);
public sealed record AdminProductSummary(int Id, string Title, decimal Price, int Stock, bool Offer, string Sku, string Seller);
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

public sealed record AdminProductDetails(
    int Id,
    Guid UserId,
    int? SubCategoryId,
    string Title,
    string Description,
    decimal Price,
    int Stock,
    bool Offer,
    string Sku,
    string[] Images,
    ProductAttributeRequest[] Attributes,
    int[] SimilarProductIds)
{
    public static AdminProductDetails From(Product product, int[] similarProductIds) =>
        new(
            product.Id,
            product.UserId,
            product.SubCategoryId,
            product.Title,
            product.Description,
            product.Price,
            product.Stock,
            product.Offer,
            product.Sku,
            product.Images.Select(image => image.FileName).ToArray(),
            product.AttributeValues.Select(value => new ProductAttributeRequest(value.AttributeDefinitionId, value.Value)).ToArray(),
            similarProductIds);
}
