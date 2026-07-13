namespace Marketplace.Api.Features.Admin.Produto.ProductImports.DownloadTemplate;

public static class DownloadProductImportTemplateEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/template", async (DownloadProductImportTemplateHandler handler) =>
            await handler.HandleAsync());
    }
}

