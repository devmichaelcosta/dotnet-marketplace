namespace Marketplace.Api.Features.Admin.Sellers.GetById;

public static class GetSellerByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async (
            Guid id,
            GetSellerByIdHandler handler,
            CancellationToken cancellationToken) =>
        {
            var seller = await handler.HandleAsync(id, cancellationToken);
            return seller is null ? Results.NotFound() : Results.Ok(seller);
        });
    }
}
