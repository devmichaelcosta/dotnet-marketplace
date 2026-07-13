using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Produto.ProductImports.DownloadFile;
using Marketplace.Api.Features.Admin.Produto.ProductImports.DownloadTemplate;
using Marketplace.Api.Features.Admin.Produto.ProductImports.GetById;
using Marketplace.Api.Features.Admin.Produto.ProductImports.SearchItems;
using Marketplace.Api.Features.Admin.Produto.ProductImports.SearchJobs;
using Marketplace.Api.Features.Admin.Produto.ProductImports.Shared;
using Marketplace.Api.Features.Admin.Produto.ProductImports.Upload;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.Produto.ProductImports;

public static class ProdutoImportAdminModule
{
    public static IServiceCollection AddProdutoImportAdminModule(this IServiceCollection services)
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

    public static IEndpointRouteBuilder MapProdutoImportAdminEndpoints(this IEndpointRouteBuilder app)
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


