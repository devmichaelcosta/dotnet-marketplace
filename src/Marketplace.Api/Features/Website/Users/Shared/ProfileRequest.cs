namespace Marketplace.Api.Features.Website.Users.Shared;

public sealed record ProfileRequest(string Name, string LastName, string? Cpf, AddressRequest[] Addresses);
