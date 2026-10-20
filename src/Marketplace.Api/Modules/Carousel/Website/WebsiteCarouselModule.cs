using Marketplace.Api.Modules.Carousel.Website.Search;

namespace Marketplace.Api.Modules.Carousel.Website;

public static class WebsiteCarouselModule
{
    public static IServiceCollection AddWebsiteCarouselModule(this IServiceCollection services)
    {
        services.AddScoped<SearchWebsiteCarouselHandler>();
        return services;
    }
}
