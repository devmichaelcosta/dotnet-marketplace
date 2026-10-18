namespace Marketplace.Api.Modules.Users.Website.Shared;

public sealed record AddressRequest(int StateId, string Street, string Cep, string Neighborhood, string City, string? Complement);
