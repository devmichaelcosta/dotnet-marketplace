using Marketplace.Api.Features.Website.Produto.CreateRating;
using Marketplace.Api.Features.Website.Produto.GetLiked;
using Marketplace.Api.Features.Website.Produto.Like;
using Marketplace.Api.Features.Website.Produto.Unlike;

namespace Marketplace.Api.Features.Website.Produto;

public static class ProdutoWebsiteModule
{
    public static IServiceCollection AddProdutoWebsiteModule(this IServiceCollection services)
    {
        services.AddScoped<LikeProductHandler>();
        services.AddScoped<UnlikeProductHandler>();
        services.AddScoped<GetLikedProductsHandler>();
        services.AddScoped<CreateProductRatingHandler>();

        return services;
    }

    public static IEndpointRouteBuilder MapProdutoWebsiteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        LikeProductEndpoint.Map(group);
        UnlikeProductEndpoint.Map(group);
        GetLikedProductsEndpoint.Map(group);
        CreateProductRatingEndpoint.Map(group);

        return app;
    }
}


