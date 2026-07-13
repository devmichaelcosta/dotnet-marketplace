namespace Marketplace.Api.Features.Website.Catalog.GetById;

public static class GetCatalogProductByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/products/{id:int}", async (
            int id,
            GetCatalogProductByIdHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, cancellationToken));
    }
}
