namespace Marketplace.Api.Modules.Users.Website.Shared;

public sealed record ProfileRequest(string Name, string LastName, string? Cpf, AddressRequest[] Addresses);
