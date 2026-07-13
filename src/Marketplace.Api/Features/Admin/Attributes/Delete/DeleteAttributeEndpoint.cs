namespace Marketplace.Api.Features.Admin.Attributes.Delete;

public static class DeleteAttributeEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:int}", async (
            int id,
            DeleteAttributeHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, cancellationToken));
    }
}
