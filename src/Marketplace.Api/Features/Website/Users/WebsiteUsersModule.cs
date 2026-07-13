using Marketplace.Api.Features.Website.Users.CreateAddress;
using Marketplace.Api.Features.Website.Users.DeleteAddress;
using Marketplace.Api.Features.Website.Users.GetProfile;
using Marketplace.Api.Features.Website.Users.UpdateAddress;
using Marketplace.Api.Features.Website.Users.UpdateProfile;

namespace Marketplace.Api.Features.Website.Users;

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
