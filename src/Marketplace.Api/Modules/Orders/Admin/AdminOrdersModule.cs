using Marketplace.Api.Domain;
using Marketplace.Api.Modules.Orders.Admin.GetById;
using Marketplace.Api.Modules.Orders.Admin.Search;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Modules.Orders.Admin;

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
