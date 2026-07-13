namespace Marketplace.Api.Features.Admin.Produto.Shared;

public sealed record ProductAdminActor(Guid? UserId, bool IsAdmin, bool IsSeller, string UserName);

