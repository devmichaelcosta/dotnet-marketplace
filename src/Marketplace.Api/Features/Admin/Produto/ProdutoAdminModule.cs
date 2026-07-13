using Marketplace.Api.Features.Admin.Produto.Create;
using Marketplace.Api.Features.Admin.Produto.Delete;
using Marketplace.Api.Features.Admin.Produto.DeleteImage;
using Marketplace.Api.Features.Admin.Produto.GetById;
using Marketplace.Api.Features.Admin.Produto.SaveSimilarProducts;
using Marketplace.Api.Features.Admin.Produto.Search;
using Marketplace.Api.Features.Admin.Produto.Shared;
using Marketplace.Api.Features.Admin.Produto.Update;
using Marketplace.Api.Infrastructure.Persistence;

namespace Marketplace.Api.Features.Admin.Produto;

public static class ProdutoAdminModule
{
    public static IServiceCollection AddProdutoAdminModule(this IServiceCollection services)
    {
        services.AddScoped<ProductAdminAccessPolicy>();
        services.AddScoped<ProductImagesWriter>();
        services.AddScoped<ProductAttributesWriter>();
        services.AddScoped<ProductDeletionPolicy>();

        services.AddScoped<SearchProductsHandler>();
        services.AddScoped<GetAdminProductByIdHandler>();
        services.AddScoped<CreateProductRequestValidator>();
        services.AddScoped<CreateProductHandler>();
        services.AddScoped<UpdateProductRequestValidator>();
        services.AddScoped<UpdateProductHandler>();
        services.AddScoped<DeleteProductHandler>();
        services.AddScoped<DeleteProductImageHandler>();
        services.AddScoped<SaveSimilarProductsHandler>();

        return services;
    }

    public static IEndpointRouteBuilder MapProdutoAdminEndpoints(this IEndpointRouteBuilder app)
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


