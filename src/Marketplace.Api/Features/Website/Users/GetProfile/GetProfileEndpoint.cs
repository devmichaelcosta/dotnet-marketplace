namespace Marketplace.Api.Features.Website.Users.GetProfile;

public static class GetProfileEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/me", async (
            HttpContext http,
            GetProfileHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(http, cancellationToken));
    }
}
