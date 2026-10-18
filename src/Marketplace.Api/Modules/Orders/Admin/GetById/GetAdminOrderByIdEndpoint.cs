namespace Marketplace.Api.Modules.Orders.Admin.GetById;

public static class GetAdminOrderByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}", async (
            int id,
            GetAdminOrderByIdHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, cancellationToken));
    }
}
