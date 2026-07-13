using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Produto.Ratings.Approve;
using Marketplace.Api.Features.Admin.Produto.Ratings.SearchPending;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.Produto.Ratings;

public static class ProdutoRatingAdminModule
{
    public static IServiceCollection AddProdutoRatingAdminModule(this IServiceCollection services)
    {
        services.AddScoped<ApproveProductRatingHandler>();
        services.AddScoped<SearchPendingProductRatingsHandler>();

        return services;
    }

    public static IEndpointRouteBuilder MapProdutoRatingAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/ratings")
            .RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole));

        ApproveProductRatingEndpoint.Map(group);
        SearchPendingProductRatingsEndpoint.Map(group);

        return app;
    }
}


