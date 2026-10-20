namespace Marketplace.Api.Modules.Produto.Website.Like;

public static class LikeProductEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{id:int}/like", async (
            int id,
            HttpContext http,
            LikeProductHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, http, cancellationToken))
            .RequireAuthorization();
    }
}

