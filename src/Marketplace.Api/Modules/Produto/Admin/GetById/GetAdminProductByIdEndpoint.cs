using Marketplace.Api.Modules.Produto.Admin.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Modules.Produto.Admin.GetById;

public static class GetAdminProductByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}", async (
            int id,
            HttpContext http,
            GetAdminProductByIdHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, http, cancellationToken));
    }
}

