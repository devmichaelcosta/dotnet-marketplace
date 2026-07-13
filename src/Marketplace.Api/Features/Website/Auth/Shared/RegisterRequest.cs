namespace Marketplace.Api.Features.Website.Auth.Shared;

public sealed record RegisterRequest(string Name, string LastName, string Login, string Password, string? Cpf);
