namespace Marketplace.Api.Features.Admin.Produto.ProductImports.DownloadFile;

public static class DownloadProductImportFileEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}/file", async (
            int id,
            DownloadProductImportFileHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, cancellationToken));
    }
}

