using Marketplace.Api.Modules.Produto.Website.GetLiked;
using Marketplace.Api.Modules.Produto.Website.Like;
using Marketplace.Api.Modules.Produto.Website.Unlike;

namespace Marketplace.Api.Modules.Produto.Website;

public static class ProdutoWebsiteModule
{
    public static IServiceCollection AddProdutoWebsiteModule(this IServiceCollection services)
    {
        services.AddScoped<LikeProductHandler>();
        services.AddScoped<UnlikeProductHandler>();
        services.AddScoped<GetLikedProductsHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapProdutoWebsiteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        LikeProductEndpoint.Map(group);
        UnlikeProductEndpoint.Map(group);
        GetLikedProductsEndpoint.Map(group);
        return app;
    }
}


