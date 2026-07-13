using Marketplace.Api.Features.Website.Users.Shared;

namespace Marketplace.Api.Features.Website.Users.UpdateAddress;

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
