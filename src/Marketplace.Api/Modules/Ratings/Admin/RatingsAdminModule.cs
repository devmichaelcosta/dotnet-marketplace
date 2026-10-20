using Marketplace.Api.Domain;
using Marketplace.Api.Modules.Ratings.Admin.Approve;
using Marketplace.Api.Modules.Ratings.Admin.SearchPending;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Modules.Ratings.Admin;

public static class RatingsAdminModule
{
    public static IServiceCollection AddRatingsAdminModule(this IServiceCollection services)
    {
        services.AddScoped<ApproveProductRatingHandler>();
        services.AddScoped<SearchPendingProductRatingsHandler>();

        return services;
    }

    public static IEndpointRouteBuilder MapRatingsAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/ratings")
            .RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole));

        ApproveProductRatingEndpoint.Map(group);
        SearchPendingProductRatingsEndpoint.Map(group);

        return app;
    }
}


