namespace Marketplace.Api.Modules.Sellers.Admin.Update;

public static class UpdateSellerEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateSellerRequest request,
            UpdateSellerHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, request, cancellationToken));
    }
}
