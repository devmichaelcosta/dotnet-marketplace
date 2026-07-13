namespace Marketplace.Api.Features.Admin.Categories.Update;

public static class UpdateCategoryEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{id:int}", async (
            int id,
            UpdateCategoryRequest request,
            UpdateCategoryHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, request, cancellationToken));
    }
}
