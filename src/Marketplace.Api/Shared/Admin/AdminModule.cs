using Marketplace.Api.Modules.Attributes.Admin;
using Marketplace.Api.Modules.Carousel.Admin;
using Marketplace.Api.Modules.Categories.Admin;
using Marketplace.Api.Modules.Sellers.Admin;
using Marketplace.Api.Modules.SubCategories.Admin;
using Marketplace.Api.Modules.Uploads.Admin;
using Marketplace.Api.Modules.Users.Admin;

namespace Marketplace.Api.Shared.Admin;

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
