using Marketplace.Api.Features.Admin.Attributes;
using Marketplace.Api.Features.Admin.Carousel;
using Marketplace.Api.Features.Admin.Categories;
using Marketplace.Api.Features.Admin.Sellers;
using Marketplace.Api.Features.Admin.SubCategories;
using Marketplace.Api.Features.Admin.Uploads;
using Marketplace.Api.Features.Admin.Users;

namespace Marketplace.Api.Features.Admin;

public static class AdminModule
{
    public static IServiceCollection AddAdminModule(this IServiceCollection services)
    {
        services.AddUsersModule();
        services.AddCategoriesModule();
        services.AddSubCategoriesModule();
        services.AddAttributesModule();
        services.AddSellersModule();
        services.AddCarouselModule();
        services.AddUploadsModule();

        return services;
    }

    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapUsersEndpoints();
        app.MapCategoriesEndpoints();
        app.MapSubCategoriesEndpoints();
        app.MapAttributesEndpoints();
        app.MapSellersEndpoints();
        app.MapCarouselEndpoints();
        app.MapUploadsEndpoints();
        return app;
    }
}
