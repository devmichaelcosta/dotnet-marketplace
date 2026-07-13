namespace Marketplace.Api.Features.Admin.Categories.Create;

public static class CreateCategoryEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            CreateCategoryRequest request,
            CreateCategoryHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(request, cancellationToken));
    }
}
