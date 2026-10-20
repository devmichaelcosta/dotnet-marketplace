using Marketplace.Api.Modules.Auth.Website.Login;
using Marketplace.Api.Modules.Auth.Website.PasswordRecovery.Confirm;
using Marketplace.Api.Modules.Auth.Website.PasswordRecovery.Request;
using Marketplace.Api.Modules.Auth.Website.Register;
using Marketplace.Api.Modules.Auth.Website.RegisterSeller;
using Marketplace.Api.Modules.Auth.Website.Session;
using Marketplace.Api.Modules.Auth.Website.Shared;

namespace Marketplace.Api.Modules.Auth.Website;

public static class WebsiteAuthModule
{
    public static IServiceCollection AddWebsiteAuthModule(this IServiceCollection services)
    {
        services.AddScoped<RegisterHandler>();
        services.AddScoped<RegisterSellerHandler>();
        services.AddScoped<LoginHandler>();
        services.AddScoped<IPasswordResetIdentity, IdentityPasswordResetIdentity>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<RequestPasswordResetHandler>();
        services.AddScoped<ResetPasswordHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapWebsiteAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        RegisterEndpoint.Map(group);
        RegisterSellerEndpoint.Map(group);
        LoginEndpoint.Map(group);
        RequestPasswordResetEndpoint.Map(group);
        ResetPasswordEndpoint.Map(group);

        var sessionGroup = app.MapGroup("/auth").WithTags("Auth Session");
        GetAntiforgeryTokenEndpoint.Map(sessionGroup);
        CreateSessionEndpoint.Map(sessionGroup);
        DeleteSessionEndpoint.Map(sessionGroup);

        return app;
    }
}
