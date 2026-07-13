using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Orders.GetById;
using Marketplace.Api.Features.Admin.Orders.Search;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.Orders;

public static class AdminOrdersModule
{
    public static IServiceCollection AddAdminOrdersModule(this IServiceCollection services)
    {
        services.AddScoped<SearchAdminOrdersHandler>();
        services.AddScoped<GetAdminOrderByIdHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapAdminOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/orders")
            .RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole))
            .WithTags("Admin Orders");

        SearchAdminOrdersEndpoint.Map(group);
        GetAdminOrderByIdEndpoint.Map(group);

        return app;
    }
}
