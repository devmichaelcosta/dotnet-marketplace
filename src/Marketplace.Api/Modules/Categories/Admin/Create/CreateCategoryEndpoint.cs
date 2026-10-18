namespace Marketplace.Api.Modules.Categories.Admin.Create;

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
