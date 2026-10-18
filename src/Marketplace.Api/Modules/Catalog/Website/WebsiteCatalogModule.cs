using Marketplace.Api.Modules.Catalog.Website.GetById;
using Marketplace.Api.Modules.Catalog.Website.GetHome;
using Marketplace.Api.Modules.Catalog.Website.GetStates;
using Marketplace.Api.Modules.Catalog.Website.SearchProducts;

namespace Marketplace.Api.Modules.Catalog.Website;

public static class WebsiteCatalogModule
{
    public static IServiceCollection AddWebsiteCatalogModule(this IServiceCollection services)
    {
        services.AddScoped<GetStatesHandler>();
        services.AddScoped<GetHomeHandler>();
        services.AddScoped<SearchProductsHandler>();
        services.AddScoped<GetCatalogProductByIdHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapWebsiteCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/catalog").WithTags("Catalog");

        GetStatesEndpoint.Map(group);
        GetHomeEndpoint.Map(group);
        SearchProductsEndpoint.Map(group);
        GetCatalogProductByIdEndpoint.Map(group);

        return app;
    }
}
