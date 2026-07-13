namespace Marketplace.Api.Features.Admin.SubCategories.Delete;

public static class DeleteSubCategoryEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:int}", async (
            int id,
            DeleteSubCategoryHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, cancellationToken));
    }
}
