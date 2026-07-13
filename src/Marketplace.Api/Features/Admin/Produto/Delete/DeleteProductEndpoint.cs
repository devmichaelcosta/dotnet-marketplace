using Marketplace.Api.Features.Admin.Produto.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Produto.Delete;

public static class DeleteProductEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:int}", async (
            int id,
            HttpContext http,
            DeleteProductHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, http, cancellationToken));
    }
}

