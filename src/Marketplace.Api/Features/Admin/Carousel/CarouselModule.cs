using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Carousel.Create;
using Marketplace.Api.Features.Admin.Carousel.Delete;
using Marketplace.Api.Features.Admin.Carousel.Search;
using Marketplace.Api.Features.Admin.Carousel.Update;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.Carousel;

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
