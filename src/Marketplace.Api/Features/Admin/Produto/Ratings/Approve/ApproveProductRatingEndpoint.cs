namespace Marketplace.Api.Features.Admin.Produto.Ratings.Approve;

public static class ApproveProductRatingEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{id:int}/approve", async (
            int id,
            ApproveProductRatingHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, cancellationToken));
    }
}

