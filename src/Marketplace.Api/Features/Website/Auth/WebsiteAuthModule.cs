using Marketplace.Api.Features.Website.Auth.Login;
using Marketplace.Api.Features.Website.Auth.Register;
using Marketplace.Api.Features.Website.Auth.RegisterSeller;

namespace Marketplace.Api.Features.Website.Auth;

public static class WebsiteAuthModule
{
    public static IServiceCollection AddWebsiteAuthModule(this IServiceCollection services)
    {
        services.AddScoped<RegisterHandler>();
        services.AddScoped<RegisterSellerHandler>();
        services.AddScoped<LoginHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapWebsiteAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        RegisterEndpoint.Map(group);
        RegisterSellerEndpoint.Map(group);
        LoginEndpoint.Map(group);

        return app;
    }
}
