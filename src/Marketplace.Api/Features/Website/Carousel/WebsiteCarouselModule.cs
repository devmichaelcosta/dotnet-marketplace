using Marketplace.Api.Features.Website.Carousel.Search;

namespace Marketplace.Api.Features.Website.Carousel;

public static class WebsiteCarouselModule
{
    public static IServiceCollection AddWebsiteCarouselModule(this IServiceCollection services)
    {
        services.AddScoped<SearchWebsiteCarouselHandler>();
        return services;
    }
}
