namespace Marketplace.Api.Features.Admin.Categories.Delete;

public static class DeleteCategoryEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:int}", async (
            int id,
            DeleteCategoryHandler handler,
            CancellationToken cancellationToken) =>
            await handler.HandleAsync(id, cancellationToken));
    }
}
