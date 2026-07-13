using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Produto.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Produto.SaveSimilarProducts;

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

