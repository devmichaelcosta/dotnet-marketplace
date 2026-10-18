using Marketplace.Api.Modules.Cart.Website.AddItem;
using Marketplace.Api.Modules.Cart.Website.Checkout;
using Marketplace.Api.Modules.Cart.Website.DeleteItem;
using Marketplace.Api.Modules.Cart.Website.GetCart;
using Marketplace.Api.Modules.Cart.Website.UpdateItem;

namespace Marketplace.Api.Modules.Cart.Website;

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
