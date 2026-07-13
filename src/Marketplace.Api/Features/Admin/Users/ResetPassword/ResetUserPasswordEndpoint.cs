namespace Marketplace.Api.Features.Admin.Users.ResetPassword;

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
