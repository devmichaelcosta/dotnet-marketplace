using Marketplace.Api.Features.Products.Admin.Create;
using Marketplace.Api.Features.Products.Admin.Delete;
using Marketplace.Api.Features.Products.Admin.DeleteImage;
using Marketplace.Api.Features.Products.Admin.GetById;
using Marketplace.Api.Features.Products.Admin.SaveSimilarProducts;
using Marketplace.Api.Features.Products.Admin.Search;
using Marketplace.Api.Features.Products.Admin.Shared;
using Marketplace.Api.Features.Products.Admin.Update;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Products.Admin;

public static class ProductAdminModule
{
    public static IServiceCollection AddProductAdminModule(this IServiceCollection services)
    {
        services.AddScoped<ProductAdminAccessPolicy>();
        services.AddScoped<ProductRequestValidator>();
        services.AddScoped<ProductImagesWriter>();
        services.AddScoped<ProductAttributesWriter>();
        services.AddScoped<ProductDeletionPolicy>();

        services.AddScoped<SearchProductsHandler>();
        services.AddScoped<GetAdminProductByIdHandler>();
        services.AddScoped<CreateProductHandler>();
        services.AddScoped<UpdateProductHandler>();
        services.AddScoped<DeleteProductHandler>();
        services.AddScoped<DeleteProductImageHandler>();
        services.AddScoped<SaveSimilarProductsHandler>();

        return services;
    }

    public static IEndpointRouteBuilder MapProductAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/products")
            .RequireAuthorization(policy => policy.RequireRole(MarketplaceSeed.AdminRole, MarketplaceSeed.SellerRole));

        SearchProductsEndpoint.Map(group);
        GetAdminProductByIdEndpoint.Map(group);
        CreateProductEndpoint.Map(group);
        UpdateProductEndpoint.Map(group);
        DeleteProductEndpoint.Map(group);
        DeleteProductImageEndpoint.Map(group);
        SaveSimilarProductsEndpoint.Map(group);

        return app;
    }
}
