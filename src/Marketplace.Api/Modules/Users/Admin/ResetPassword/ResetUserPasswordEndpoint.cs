namespace Marketplace.Api.Modules.Users.Admin.ResetPassword;

public static class ResetUserPasswordEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/reset-password", async (
            Guid id,
            ResetUserPasswordRequest request,
            ResetUserPasswordHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, request, cancellationToken));
    }
}
