using Marketplace.Api.Domain;

namespace Marketplace.Api.Features.Admin.SubCategories.Search;

public sealed record SubCategoryResponse(int Id, int CategoryId, string Title, string Category)
{
    public static SubCategoryResponse From(SubCategory subCategory) =>
        new(subCategory.Id, subCategory.CategoryId, subCategory.Title, subCategory.Category?.Title ?? string.Empty);
}
