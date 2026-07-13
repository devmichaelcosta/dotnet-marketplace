namespace Marketplace.Api.Features.Website.Produto.Unlike;

public static class UnlikeProductEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:int}/like", async (
            int id,
            HttpContext http,
            UnlikeProductHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, http, cancellationToken))
            .RequireAuthorization();
    }
}

