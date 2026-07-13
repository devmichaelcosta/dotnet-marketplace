using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Categories.Create;
using Marketplace.Api.Features.Admin.Categories.Delete;
using Marketplace.Api.Features.Admin.Categories.Search;
using Marketplace.Api.Features.Admin.Categories.Update;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.Categories;

public static class CategoriesModule
{
    public static IServiceCollection AddCategoriesModule(this IServiceCollection services)
    {
        services.AddScoped<SearchCategoriesHandler>();
        services.AddScoped<CreateCategoryHandler>();
        services.AddScoped<UpdateCategoryHandler>();
        services.AddScoped<DeleteCategoryHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapCategoriesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/categories")
            .RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole));

        SearchCategoriesEndpoint.Map(group);
        CreateCategoryEndpoint.Map(group);
        UpdateCategoryEndpoint.Map(group);
        DeleteCategoryEndpoint.Map(group);
        return app;
    }
}
