namespace Marketplace.Api.Features.Admin.Users.Create;

public sealed record CreateUserRequest(string Name, string LastName, string Login, string? Password, string? Cpf, string Role);
