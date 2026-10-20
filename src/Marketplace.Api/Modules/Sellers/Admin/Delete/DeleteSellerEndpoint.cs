namespace Marketplace.Api.Modules.Sellers.Admin.Delete;

public static class DeleteSellerEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeleteSellerHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, cancellationToken));
    }
}
