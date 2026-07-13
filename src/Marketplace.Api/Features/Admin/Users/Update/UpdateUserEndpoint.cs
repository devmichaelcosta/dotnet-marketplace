namespace Marketplace.Api.Features.Admin.Users.Update;

public static class UpdateUserEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateUserRequest request,
            UpdateUserHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, request, cancellationToken));
    }
}
