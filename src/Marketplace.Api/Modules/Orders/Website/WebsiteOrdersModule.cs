using Marketplace.Api.Modules.Orders.Website.GetById;
using Marketplace.Api.Modules.Orders.Website.Search;

namespace Marketplace.Api.Modules.Orders.Website;

public static class WebsiteOrdersModule
{
    public static IServiceCollection AddWebsiteOrdersModule(this IServiceCollection services)
    {
        services.AddScoped<SearchOrdersHandler>();
        services.AddScoped<GetOrderByIdHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapWebsiteOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").RequireAuthorization().WithTags("Orders");

        SearchOrdersEndpoint.Map(group);
        GetOrderByIdEndpoint.Map(group);

        return app;
    }
}
