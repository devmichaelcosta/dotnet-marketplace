namespace Marketplace.Api.Features.Website.Orders.GetById;

public static class GetOrderByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}", async (
            int id,
            HttpContext http,
            GetOrderByIdHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, http, cancellationToken));
    }
}
