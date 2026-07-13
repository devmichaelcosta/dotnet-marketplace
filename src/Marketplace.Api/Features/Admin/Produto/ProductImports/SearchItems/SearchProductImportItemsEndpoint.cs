namespace Marketplace.Api.Features.Admin.Produto.ProductImports.SearchItems;

public static class SearchProductImportItemsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}/items", async (
            [AsParameters] SearchProductImportItemsQuery query,
            SearchProductImportItemsHandler handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(query, cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });
    }
}

