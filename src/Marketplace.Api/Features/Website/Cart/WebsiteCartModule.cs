using Marketplace.Api.Features.Website.Cart.AddItem;
using Marketplace.Api.Features.Website.Cart.Checkout;
using Marketplace.Api.Features.Website.Cart.DeleteItem;
using Marketplace.Api.Features.Website.Cart.GetCart;
using Marketplace.Api.Features.Website.Cart.UpdateItem;

namespace Marketplace.Api.Features.Website.Cart;

public static class WebsiteCartModule
{
    public static IServiceCollection AddWebsiteCartModule(this IServiceCollection services)
    {
        services.AddScoped<GetCartHandler>();
        services.AddScoped<AddCartItemHandler>();
        services.AddScoped<UpdateCartItemHandler>();
        services.AddScoped<DeleteCartItemHandler>();
        services.AddScoped<CheckoutHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapWebsiteCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cart").WithTags("Cart");

        GetCartEndpoint.Map(group);
        AddCartItemEndpoint.Map(group);
        UpdateCartItemEndpoint.Map(group);
        DeleteCartItemEndpoint.Map(group);
        CheckoutEndpoint.Map(group);

        return app;
    }
}
