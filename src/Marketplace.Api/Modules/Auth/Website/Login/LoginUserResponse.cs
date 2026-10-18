namespace Marketplace.Api.Modules.Auth.Website.Login;

public sealed record LoginUserResponse(Guid Id, string UserName, string Name, string[] Roles);
