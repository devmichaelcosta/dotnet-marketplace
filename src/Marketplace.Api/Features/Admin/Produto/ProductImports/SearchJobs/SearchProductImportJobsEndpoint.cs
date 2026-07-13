namespace Marketplace.Api.Features.Admin.Produto.ProductImports.SearchJobs;

public static class SearchProductImportJobsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            [AsParameters] SearchProductImportJobsQuery query,
            SearchProductImportJobsHandler handler,
            CancellationToken cancellationToken) =>
            Results.Ok(await handler.HandleAsync(query, cancellationToken)));
    }
}

