namespace Marketplace.Api.Modules.Users.Admin.Update;

public sealed record UpdateUserRequest(string Name, string LastName, string Login, string? Password, string? Cpf, string Role);
