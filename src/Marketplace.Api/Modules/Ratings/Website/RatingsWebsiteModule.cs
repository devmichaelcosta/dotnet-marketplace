using Marketplace.Api.Modules.Ratings.Website.CreateRating;

namespace Marketplace.Api.Modules.Ratings.Website;

public static class RatingsWebsiteModule
{
    public static IServiceCollection AddRatingsWebsiteModule(this IServiceCollection services)
    {
        services.AddScoped<CreateProductRatingHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapRatingsWebsiteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");
        CreateProductRatingEndpoint.Map(group);
        return app;
    }
}
