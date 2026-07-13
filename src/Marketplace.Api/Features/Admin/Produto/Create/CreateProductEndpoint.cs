namespace Marketplace.Api.Features.Admin.Produto.Create;

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

