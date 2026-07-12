using Marketplace.Api.Domain;
using Marketplace.Api.Features.Products.Admin.Shared;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Products.Admin.Create;

public sealed class CreateProductHandler(
    MarketplaceDbContext db,
    ProductAdminAccessPolicy accessPolicy,
    CreateProductRequestValidator validator,
    ProductImagesWriter imagesWriter,
    ProductAttributesWriter attributesWriter)
{
    public async Task<IResult> HandleAsync(CreateProductRequest request, HttpContext http, CancellationToken cancellationToken)
    {
        var errors = validator.Validate(request);
        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        var actor = accessPolicy.ResolveActor(http);
        var ownerId = accessPolicy.ResolveOwnerId(request, actor);
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
            CreatedBy = actor.UserName,
            Images = imagesWriter.Build(request.Images),
            AttributeValues = attributesWriter.Build(request.Attributes)
        };

        db.Products.Add(product);
        await db.SaveChangesAsync(cancellationToken);
        return Results.Created($"/api/admin/products/{product.Id}", new { product.Id });
    }
}
