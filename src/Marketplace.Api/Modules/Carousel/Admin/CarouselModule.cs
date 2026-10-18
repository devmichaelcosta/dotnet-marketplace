using Marketplace.Api.Domain;
using Marketplace.Api.Modules.Carousel.Admin.Create;
using Marketplace.Api.Modules.Carousel.Admin.Delete;
using Marketplace.Api.Modules.Carousel.Admin.Search;
using Marketplace.Api.Modules.Carousel.Admin.Update;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Modules.Carousel.Admin;

public static class CarouselModule
{
    public static IServiceCollection AddCarouselModule(this IServiceCollection services)
    {
        services.AddScoped<SearchCarouselHandler>();
        services.AddScoped<CreateCarouselHandler>();
        services.AddScoped<UpdateCarouselHandler>();
        services.AddScoped<DeleteCarouselHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapCarouselEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/carousel")
            .RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole));

        SearchCarouselEndpoint.Map(group);
        CreateCarouselEndpoint.Map(group);
        UpdateCarouselEndpoint.Map(group);
        DeleteCarouselEndpoint.Map(group);
        return app;
    }
}
