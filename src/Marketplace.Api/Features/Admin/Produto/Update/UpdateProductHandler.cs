using Marketplace.Api.Features.Admin.Produto.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Produto.Update;

public sealed class UpdateProductHandler(
    MarketplaceDbContext db,
    ProductAdminAccessPolicy accessPolicy,
    UpdateProductRequestValidator validator,
    ProductImagesWriter imagesWriter,
    ProductAttributesWriter attributesWriter)
{
    public async Task<IResult> HandleAsync(int id, UpdateProductRequest request, HttpContext http, CancellationToken cancellationToken)
    {
        var errors = validator.Validate(request);
        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        var product = await db.Products
            .Include(item => item.Images)
            .Include(item => item.AttributeValues)
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

        product.SubCategoryId = request.SubCategoryId;
        product.Title = request.Title.Trim();
        product.Description = request.Description.Trim();
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.Offer = request.Offer;
        product.Sku = request.Sku.Trim();

        imagesWriter.Replace(product, request.Images, db);
        attributesWriter.Replace(
            product,
            request.Attributes.Select(attribute => (attribute.AttributeId, attribute.Value)),
            db);

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Results.Ok(new { product.Id });
    }
}

