using Marketplace.Api.Features.Website.Users.Shared;

namespace Marketplace.Api.Features.Website.Users.UpdateProfile;

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
