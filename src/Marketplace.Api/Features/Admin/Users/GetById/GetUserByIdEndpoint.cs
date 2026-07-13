namespace Marketplace.Api.Features.Admin.Users.GetById;

public static class GetUserByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async (
            Guid id,
            GetUserByIdHandler handler) =>
        {
            var user = await handler.HandleAsync(id);
            return user is null ? Results.NotFound() : Results.Ok(user);
        });
    }
}
