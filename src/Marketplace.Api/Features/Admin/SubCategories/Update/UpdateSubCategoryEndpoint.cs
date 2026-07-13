namespace Marketplace.Api.Features.Admin.SubCategories.Update;

public static class UpdateSubCategoryEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{id:int}", async (
            int id,
            UpdateSubCategoryRequest request,
            UpdateSubCategoryHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, request, cancellationToken));
    }
}
