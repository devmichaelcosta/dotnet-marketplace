using Marketplace.Api.Features.Admin.Produto.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Produto.GetById;

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

