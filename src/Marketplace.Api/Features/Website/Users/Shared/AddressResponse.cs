using Marketplace.Api.Domain;

namespace Marketplace.Api.Features.Website.Users.Shared;

public sealed record AddressResponse(Guid Id, int StateId, string State, string Street, string Cep, string Neighborhood, string City, string? Complement)
{
    public static AddressResponse From(Address address) =>
        new(address.Id, address.StateId, address.State?.Abbreviation ?? string.Empty, address.Street, address.Cep, address.Neighborhood, address.City, address.Complement);
}
