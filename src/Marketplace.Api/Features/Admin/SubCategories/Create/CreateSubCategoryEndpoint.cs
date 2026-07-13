namespace Marketplace.Api.Features.Admin.SubCategories.Create;

public static class CreateSubCategoryEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            CreateSubCategoryRequest request,
            CreateSubCategoryHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(request, cancellationToken));
    }
}
