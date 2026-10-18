namespace Marketplace.Api.Modules.Users.Admin.Create;

public sealed record CreateUserRequest(string Name, string LastName, string Login, string? Password, string? Cpf, string Role);
