using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Users.Create;
using Marketplace.Api.Features.Admin.Users.Delete;
using Marketplace.Api.Features.Admin.Users.GetById;
using Marketplace.Api.Features.Admin.Users.ResetPassword;
using Marketplace.Api.Features.Admin.Users.Search;
using Marketplace.Api.Features.Admin.Users.Update;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.Users;

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
