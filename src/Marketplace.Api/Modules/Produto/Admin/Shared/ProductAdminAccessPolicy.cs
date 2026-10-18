using Marketplace.Api.Domain;
using Marketplace.Api.Infrastructure.Security;
using Marketplace.Api.Modules.Produto.Admin.Create;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Modules.Produto.Admin.Shared;

public sealed class ProductAdminAccessPolicy
{
    public ProductAdminActor ResolveActor(HttpContext http) =>
        new(
            http.User.GetUserId(),
            http.User.IsInRole(MarketplaceSeed.AdminRole),
            http.User.IsInRole(MarketplaceSeed.SellerRole),
            http.User.Identity?.Name ?? "system");

    public Guid? ResolveOwnerId(CreateProductRequest request, ProductAdminActor actor) =>
        actor.IsAdmin && request.UserId is not null
            ? request.UserId.Value
            : actor.UserId;

    public bool CanManage(Product product, ProductAdminActor actor) =>
        actor.IsAdmin || (actor.IsSeller && product.UserId == actor.UserId);
}

