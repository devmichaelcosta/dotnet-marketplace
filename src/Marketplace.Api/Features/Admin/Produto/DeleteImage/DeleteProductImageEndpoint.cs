using Marketplace.Api.Features.Admin.Produto.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Produto.DeleteImage;

public static class DeleteProductImageEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:int}/images/{fileName}", async (
            int id,
            string fileName,
            HttpContext http,
            DeleteProductImageHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, fileName, http, cancellationToken));
    }
}

