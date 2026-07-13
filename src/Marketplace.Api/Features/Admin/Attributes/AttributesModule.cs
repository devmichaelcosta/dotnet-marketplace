using Marketplace.Api.Features.Admin.Attributes.Create;
using Marketplace.Api.Features.Admin.Attributes.Delete;
using Marketplace.Api.Features.Admin.Attributes.Search;
using Marketplace.Api.Features.Admin.Attributes.Update;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.Attributes;

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
