using Marketplace.Api.Domain;
using Marketplace.Api.Modules.Produto.Admin.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Modules.Produto.Admin.SaveSimilarProducts;

public static class SaveSimilarProductsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{id:int}/similar-products", async (
            int id,
            SaveSimilarProductsRequest request,
            SaveSimilarProductsHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, request, cancellationToken));
    }
}

