using Marketplace.Api.Modules.Attributes.Admin.Create;
using Marketplace.Api.Modules.Attributes.Admin.Delete;
using Marketplace.Api.Modules.Attributes.Admin.Search;
using Marketplace.Api.Modules.Attributes.Admin.Update;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Modules.Attributes.Admin;

public static class AttributesModule
{
    public static IServiceCollection AddAttributesModule(this IServiceCollection services)
    {
        services.AddScoped<SearchAttributesHandler>();
        services.AddScoped<CreateAttributeHandler>();
        services.AddScoped<UpdateAttributeHandler>();
        services.AddScoped<DeleteAttributeHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapAttributesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/attributes")
            .RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole));

        SearchAttributesEndpoint.Map(group);
        CreateAttributeEndpoint.Map(group);
        UpdateAttributeEndpoint.Map(group);
        DeleteAttributeEndpoint.Map(group);
        return app;
    }
}
