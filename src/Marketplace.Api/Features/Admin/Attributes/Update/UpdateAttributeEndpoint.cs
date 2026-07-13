namespace Marketplace.Api.Features.Admin.Attributes.Update;

public static class UpdateAttributeEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{id:int}", async (
            int id,
            UpdateAttributeRequest request,
            UpdateAttributeHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, request, cancellationToken));
    }
}
