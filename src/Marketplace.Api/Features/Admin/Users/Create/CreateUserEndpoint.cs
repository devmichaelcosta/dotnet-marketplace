namespace Marketplace.Api.Features.Admin.Users.Create;

public static class CreateUserEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            CreateUserRequest request,
            CreateUserHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(request, cancellationToken));
    }
}
