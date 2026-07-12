using Marketplace.Api.Features.Products.Admin.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Products.Admin.DeleteImage;

public static class DeleteProductImageEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:int}/images/{fileName}", async (
            int id,
            string fileName,
            HttpContext http,
            DeleteProductImageHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, fileName, http, cancellationToken));
    }
}

public sealed class DeleteProductImageHandler(
    MarketplaceDbContext db,
    ProductAdminAccessPolicy accessPolicy,
    IWebHostEnvironment environment)
{
    public async Task<IResult> HandleAsync(int id, string fileName, HttpContext http, CancellationToken cancellationToken)
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

        var sanitized = Path.GetFileName(fileName);
        var image = product.Images.FirstOrDefault(item =>
            string.Equals(ProductImageStorage.NormalizeFileName(item.FileName), sanitized, StringComparison.OrdinalIgnoreCase));
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
    }
}
