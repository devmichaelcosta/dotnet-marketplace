namespace Marketplace.Api.Modules.Auth.Website.Login;

public sealed record LoginResponse(string Token, LoginUserResponse User);
