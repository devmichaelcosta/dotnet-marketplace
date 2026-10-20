using Marketplace.Api.Domain;
using Marketplace.Api.Modules.Uploads.Admin.Create;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Modules.Uploads.Admin;

public static class UploadsModule
{
    public static IServiceCollection AddUploadsModule(this IServiceCollection services)
    {
        services.AddScoped<CreateUploadHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapUploadsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/admin/uploads/{scope}", async (
            string scope,
            IFormFile file,
            CreateUploadHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(scope, file, cancellationToken))
            .RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole, MarketplaceSeed.SellerRole))
            .DisableAntiforgery();

        return app;
    }
}
