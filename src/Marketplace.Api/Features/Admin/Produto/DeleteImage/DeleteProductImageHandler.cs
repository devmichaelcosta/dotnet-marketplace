using Marketplace.Api.Features.Admin.Produto.Shared;
using Marketplace.Api.Features.Website.Produto.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Produto.DeleteImage;

public sealed class DeleteProductImageHandler(
    MarketplaceDbContext db,
    ProductAdminAccessPolicy accessPolicy,
    IWebHostEnvironment environment,
    ILogger<DeleteProductImageHandler> logger)
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

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        db.ProductImages.Remove(image);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var basePath = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        var absolutePath = Path.Combine(basePath, "uploads", "products", sanitized);
        try
        {
            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Falha ao remover arquivo fisico da imagem {FileName} do produto {ProductId}.", sanitized, id);
        }

        return Results.NoContent();
    }
}


