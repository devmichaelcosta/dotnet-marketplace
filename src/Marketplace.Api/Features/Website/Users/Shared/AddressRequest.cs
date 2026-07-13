namespace Marketplace.Api.Features.Website.Users.Shared;

public sealed record AddressRequest(int StateId, string Street, string Cep, string Neighborhood, string City, string? Complement);
