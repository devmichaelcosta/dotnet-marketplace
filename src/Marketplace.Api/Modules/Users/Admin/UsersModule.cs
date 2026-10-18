using Marketplace.Api.Domain;
using Marketplace.Api.Modules.Users.Admin.Create;
using Marketplace.Api.Modules.Users.Admin.Delete;
using Marketplace.Api.Modules.Users.Admin.GetById;
using Marketplace.Api.Modules.Users.Admin.ResetPassword;
using Marketplace.Api.Modules.Users.Admin.Search;
using Marketplace.Api.Modules.Users.Admin.Update;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Modules.Users.Admin;

public static class UsersModule
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services)
    {
        services.AddScoped<SearchUsersHandler>();
        services.AddScoped<GetUserByIdHandler>();
        services.AddScoped<CreateUserHandler>();
        services.AddScoped<UpdateUserHandler>();
        services.AddScoped<ResetUserPasswordHandler>();
        services.AddScoped<DeleteUserHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/users")
            .RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole));

        SearchUsersEndpoint.Map(group);
        GetUserByIdEndpoint.Map(group);
        CreateUserEndpoint.Map(group);
        UpdateUserEndpoint.Map(group);
        ResetUserPasswordEndpoint.Map(group);
        DeleteUserEndpoint.Map(group);
        return app;
    }
}
