namespace Marketplace.Api.Features.Admin.Produto.ProductImports.Upload;

public static class UploadProductImportEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            IFormFile file,
            HttpContext http,
            UploadProductImportHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(file, http, cancellationToken))
            .DisableAntiforgery();
    }
}

