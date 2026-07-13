using Marketplace.Api.Domain;

namespace Marketplace.Api.Features.Admin.Categories.Search;

public sealed record CategoryResponse(int Id, string Title, string? Image, SubCategoryOptionResponse[] SubCategories)
{
    public static CategoryResponse From(Category category) =>
        new(
            category.Id,
            category.Title,
            category.Image,
            category.SubCategories
                .OrderBy(subCategory => subCategory.Title)
                .Select(subCategory => new SubCategoryOptionResponse(subCategory.Id, subCategory.Title))
                .ToArray());
}
