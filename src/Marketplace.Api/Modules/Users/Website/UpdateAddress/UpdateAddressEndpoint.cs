using Marketplace.Api.Modules.Users.Website.Shared;

namespace Marketplace.Api.Modules.Users.Website.UpdateAddress;

public static class UpdateAddressEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/me/addresses/{id:guid}", async (
            Guid id,
            AddressRequest request,
            HttpContext http,
            UpdateAddressHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, request, http, cancellationToken));
    }
}
