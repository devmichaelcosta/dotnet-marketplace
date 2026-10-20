namespace Marketplace.Api.Modules.Sellers.Admin.Create;

public static class CreateSellerEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            CreateSellerRequest request,
            CreateSellerHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(request, cancellationToken));
    }
}
