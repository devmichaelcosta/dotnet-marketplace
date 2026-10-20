namespace Marketplace.Api.Modules.Produto.Admin.Shared;

public sealed record ProductAdminActor(Guid? UserId, bool IsAdmin, bool IsSeller, string UserName);

