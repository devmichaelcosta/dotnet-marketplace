namespace Marketplace.Api.Features.Admin.Users.Update;

public sealed record UpdateUserRequest(string Name, string LastName, string Login, string? Password, string? Cpf, string Role);
