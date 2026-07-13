using Marketplace.Api.Features.Website.Users.Shared;

namespace Marketplace.Api.Features.Website.Users.CreateAddress;

public static class CreateAddressEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/me/addresses", async (
            AddressRequest request,
            HttpContext http,
            CreateAddressHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(request, http, cancellationToken));
    }
}
