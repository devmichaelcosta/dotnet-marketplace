namespace Marketplace.Api.Modules.Users.Admin.Delete;

public static class DeleteUserEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeleteUserHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, cancellationToken));
    }
}
