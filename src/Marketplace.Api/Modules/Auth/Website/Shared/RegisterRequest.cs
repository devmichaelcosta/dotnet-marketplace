namespace Marketplace.Api.Modules.Auth.Website.Shared;

public sealed record RegisterRequest(string Name, string LastName, string Login, string Password, string? Cpf);
