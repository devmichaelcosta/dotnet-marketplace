using Marketplace.Api.Features.Website.Catalog.GetById;
using Marketplace.Api.Features.Website.Catalog.GetHome;
using Marketplace.Api.Features.Website.Catalog.GetStates;
using Marketplace.Api.Features.Website.Catalog.SearchProducts;

namespace Marketplace.Api.Features.Website.Catalog;

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
