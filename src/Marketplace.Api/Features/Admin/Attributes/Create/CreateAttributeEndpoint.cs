namespace Marketplace.Api.Features.Admin.Attributes.Create;

public static class CreateAttributeEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            CreateAttributeRequest request,
            CreateAttributeHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(request, cancellationToken));
    }
}
