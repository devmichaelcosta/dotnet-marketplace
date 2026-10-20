using Marketplace.Api.Modules.Users.Website.CreateAddress;
using Marketplace.Api.Modules.Users.Website.DeleteAddress;
using Marketplace.Api.Modules.Users.Website.GetProfile;
using Marketplace.Api.Modules.Users.Website.UpdateAddress;
using Marketplace.Api.Modules.Users.Website.UpdateProfile;

namespace Marketplace.Api.Modules.Users.Website;

public static class WebsiteUsersModule
{
    public static IServiceCollection AddWebsiteUsersModule(this IServiceCollection services)
    {
        services.AddScoped<GetProfileHandler>();
        services.AddScoped<UpdateProfileHandler>();
        services.AddScoped<CreateAddressHandler>();
        services.AddScoped<UpdateAddressHandler>();
        services.AddScoped<DeleteAddressHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapWebsiteUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").RequireAuthorization().WithTags("Users");

        GetProfileEndpoint.Map(group);
        UpdateProfileEndpoint.Map(group);
        CreateAddressEndpoint.Map(group);
        UpdateAddressEndpoint.Map(group);
        DeleteAddressEndpoint.Map(group);

        return app;
    }
}
