using Marketplace.Api.Domain;
using Marketplace.Api.Modules.Sellers.Admin.Create;
using Marketplace.Api.Modules.Sellers.Admin.Delete;
using Marketplace.Api.Modules.Sellers.Admin.GetById;
using Marketplace.Api.Modules.Sellers.Admin.Search;
using Marketplace.Api.Modules.Sellers.Admin.Update;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Modules.Sellers.Admin;

public static class SellersModule
{
    public static IServiceCollection AddSellersModule(this IServiceCollection services)
    {
        services.AddScoped<SearchSellersHandler>();
        services.AddScoped<GetSellerByIdHandler>();
        services.AddScoped<CreateSellerHandler>();
        services.AddScoped<UpdateSellerHandler>();
        services.AddScoped<DeleteSellerHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapSellersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/sellers")
            .RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole));

        SearchSellersEndpoint.Map(group);
        GetSellerByIdEndpoint.Map(group);
        CreateSellerEndpoint.Map(group);
        UpdateSellerEndpoint.Map(group);
        DeleteSellerEndpoint.Map(group);
        return app;
    }
}
