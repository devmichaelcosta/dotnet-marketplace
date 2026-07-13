using Marketplace.Api.Features.Website.Produto.Shared;
using Marketplace.Api.Features.Admin.Produto.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Produto.Delete;

public sealed class DeleteProductHandler(
    MarketplaceDbContext db,
    ProductAdminAccessPolicy accessPolicy,
    ProductDeletionPolicy deletionPolicy,
    IWebHostEnvironment environment,
    ILogger<DeleteProductHandler> logger)
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

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        await db.CartItems.Where(item => item.ProductId == id).ExecuteDeleteAsync(cancellationToken);
        await db.SimilarProducts.Where(item => item.ParentProductId == id || item.ChildProductId == id).ExecuteDeleteAsync(cancellationToken);
        await db.ProductLikes.Where(item => item.ProductId == id).ExecuteDeleteAsync(cancellationToken);
        await db.ProductRatings.Where(item => item.ProductId == id).ExecuteDeleteAsync(cancellationToken);
        await db.ProductAttributeValues.Where(item => item.ProductId == id).ExecuteDeleteAsync(cancellationToken);
        await db.ProductImages.Where(item => item.ProductId == id).ExecuteDeleteAsync(cancellationToken);
        db.Products.Remove(product);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var basePath = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        foreach (var imageName in product.Images
            .Select(image => ProductImageStorage.NormalizeFileName(image.FileName))
            .Where(imageName => !string.IsNullOrWhiteSpace(imageName)))
        {
            try
            {
                var absolutePath = Path.Combine(basePath, "uploads", "products", imageName!);
                if (File.Exists(absolutePath))
                {
                    File.Delete(absolutePath);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Falha ao remover arquivo fisico da imagem {FileName} do produto {ProductId}.", imageName, id);
            }
        }

        return Results.NoContent();
    }
}




