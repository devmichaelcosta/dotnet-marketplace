using Marketplace.Api.Features.Products.Admin.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Products.Admin.Delete;

public static class DeleteProductEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:int}", async (
            int id,
            HttpContext http,
            DeleteProductHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, http, cancellationToken));
    }
}

public sealed class DeleteProductHandler(
    MarketplaceDbContext db,
    ProductAdminAccessPolicy accessPolicy,
    ProductDeletionPolicy deletionPolicy,
    IWebHostEnvironment environment)
{
    public async Task<IResult> HandleAsync(int id, HttpContext http, CancellationToken cancellationToken)
    {
        var product = await db.Products
            .Include(item => item.Images)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (product is null)
        {
            return Results.NotFound();
        }

        var actor = accessPolicy.ResolveActor(http);
        if (!accessPolicy.CanManage(product, actor))
        {
            return Results.Forbid();
        }

        var validation = await deletionPolicy.ValidateAsync(db, product.Id, cancellationToken);
        if (validation is not null)
        {
            return validation;
        }

        await db.CartItems.Where(item => item.ProductId == id).ExecuteDeleteAsync(cancellationToken);
        await db.SimilarProducts.Where(item => item.ParentProductId == id || item.ChildProductId == id).ExecuteDeleteAsync(cancellationToken);
        await db.ProductLikes.Where(item => item.ProductId == id).ExecuteDeleteAsync(cancellationToken);
        await db.ProductRatings.Where(item => item.ProductId == id).ExecuteDeleteAsync(cancellationToken);
        await db.ProductAttributeValues.Where(item => item.ProductId == id).ExecuteDeleteAsync(cancellationToken);
        await db.ProductImages.Where(item => item.ProductId == id).ExecuteDeleteAsync(cancellationToken);
        db.Products.Remove(product);
        await db.SaveChangesAsync(cancellationToken);

        var basePath = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        foreach (var imageName in product.Images
            .Select(image => ProductImageStorage.NormalizeFileName(image.FileName))
            .Where(imageName => !string.IsNullOrWhiteSpace(imageName)))
        {
            var absolutePath = Path.Combine(basePath, "uploads", "products", imageName!);
            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
            }
        }

        return Results.NoContent();
    }
}
