using Marketplace.Api.Domain;
using Marketplace.Api.Modules.ProductImports.Admin.DownloadFile;
using Marketplace.Api.Modules.ProductImports.Admin.DownloadTemplate;
using Marketplace.Api.Modules.ProductImports.Admin.GetById;
using Marketplace.Api.Modules.ProductImports.Admin.SearchItems;
using Marketplace.Api.Modules.ProductImports.Admin.SearchJobs;
using Marketplace.Api.Modules.ProductImports.Admin.Shared;
using Marketplace.Api.Modules.ProductImports.Admin.Upload;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Modules.ProductImports.Admin;

public static class ProductImportsAdminModule
{
    public static IServiceCollection AddProductImportsAdminModule(this IServiceCollection services)
    {
        services.AddHttpClient("product-import-images", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(5);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("DotNetMarketplaceProductImport/1.0");
        });

        services.AddSingleton<ProductImportQueue>();
        services.AddScoped<ProductImportProcessor>();
        services.AddScoped<ProductImportImageDownloader>();
        services.AddScoped<DownloadProductImportTemplateHandler>();
        services.AddScoped<UploadProductImportValidator>();
        services.AddScoped<UploadProductImportHandler>();
        services.AddScoped<SearchProductImportJobsHandler>();
        services.AddScoped<GetProductImportJobByIdHandler>();
        services.AddScoped<SearchProductImportItemsHandler>();
        services.AddScoped<DownloadProductImportFileHandler>();
        services.AddHostedService<ProductImportWorker>();

        return services;
    }

    public static IEndpointRouteBuilder MapProductImportsAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/product-imports")
            .RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole))
            .WithTags("Product imports");

        DownloadProductImportTemplateEndpoint.Map(group);
        UploadProductImportEndpoint.Map(group);
        SearchProductImportJobsEndpoint.Map(group);
        GetProductImportJobByIdEndpoint.Map(group);
        SearchProductImportItemsEndpoint.Map(group);
        DownloadProductImportFileEndpoint.Map(group);

        return app;
    }
}


