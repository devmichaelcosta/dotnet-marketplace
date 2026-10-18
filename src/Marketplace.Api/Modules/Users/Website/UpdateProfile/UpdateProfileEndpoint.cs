using Marketplace.Api.Modules.Users.Website.Shared;

namespace Marketplace.Api.Modules.Users.Website.UpdateProfile;

public static class UpdateProfileEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/me", async (
            ProfileRequest request,
            HttpContext http,
            UpdateProfileHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(request, http, cancellationToken));
    }
}
