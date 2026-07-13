namespace Marketplace.Api.Features.Website.Users.DeleteAddress;

public static class DeleteAddressEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/me/addresses/{id:guid}", async (
            Guid id,
            HttpContext http,
            DeleteAddressHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, http, cancellationToken));
    }
}
