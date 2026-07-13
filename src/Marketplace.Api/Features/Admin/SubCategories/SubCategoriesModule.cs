using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.SubCategories.Create;
using Marketplace.Api.Features.Admin.SubCategories.Delete;
using Marketplace.Api.Features.Admin.SubCategories.Search;
using Marketplace.Api.Features.Admin.SubCategories.Update;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.SubCategories;

public static class SubCategoriesModule
{
    public static IServiceCollection AddSubCategoriesModule(this IServiceCollection services)
    {
        services.AddScoped<SearchSubCategoriesHandler>();
        services.AddScoped<CreateSubCategoryHandler>();
        services.AddScoped<UpdateSubCategoryHandler>();
        services.AddScoped<DeleteSubCategoryHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapSubCategoriesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/subcategories")
            .RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole));

        SearchSubCategoriesEndpoint.Map(group);
        CreateSubCategoryEndpoint.Map(group);
        UpdateSubCategoryEndpoint.Map(group);
        DeleteSubCategoryEndpoint.Map(group);
        return app;
    }
}
