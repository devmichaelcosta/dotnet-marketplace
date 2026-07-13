namespace Marketplace.Api.Features.Admin.Produto.ProductImports.GetById;

public static class GetProductImportJobByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}", async (
            int id,
            GetProductImportJobByIdHandler handler,
            CancellationToken cancellationToken) =>
        {
            var job = await handler.HandleAsync(id, cancellationToken);
            return job is null ? Results.NotFound() : Results.Ok(job);
        });
    }
}

