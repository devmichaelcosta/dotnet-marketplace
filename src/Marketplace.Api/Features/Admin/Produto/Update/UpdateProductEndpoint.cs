namespace Marketplace.Api.Features.Admin.Produto.Update;

public static class UpdateProductEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{id:int}", async (
            int id,
            UpdateProductRequest request,
            HttpContext http,
            UpdateProductHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, request, http, cancellationToken));
    }
}
