namespace Marketplace.Api.Features.Admin.Sellers.Update;

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
