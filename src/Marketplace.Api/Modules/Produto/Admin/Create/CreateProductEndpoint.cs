namespace Marketplace.Api.Modules.Produto.Admin.Create;

public static class CreateProductEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            CreateProductRequest request,
            HttpContext http,
            CreateProductHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(request, http, cancellationToken));
    }
}

