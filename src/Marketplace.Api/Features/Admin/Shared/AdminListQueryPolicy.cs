using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Categories.Search;
using Marketplace.Api.Features.Admin.Users.Search;

namespace Marketplace.Api.Features.Admin.Shared;

public static class AdminListQueryPolicy
{
    public static IQueryable<ApplicationUser> ApplyUserSort(IQueryable<ApplicationUser> query, string? sort, string? direction) =>
        (sort?.ToLowerInvariant(), IsAscending(direction)) switch
        {
            ("login", true) => query.OrderBy(user => user.UserName),
            ("login", false) => query.OrderByDescending(user => user.UserName),
            ("cpf", true) => query.OrderBy(user => user.Cpf).ThenBy(user => user.Name),
            ("cpf", false) => query.OrderByDescending(user => user.Cpf).ThenByDescending(user => user.Name),
            ("name", false) => query.OrderByDescending(user => user.Name).ThenByDescending(user => user.LastName),
            _ => query.OrderBy(user => user.Name).ThenBy(user => user.LastName)
        };

    public static List<UserResponse> ApplyUserResponseSort(IEnumerable<UserResponse> users, string? sort, string? direction)
    {
        var query = users.AsEnumerable();
        return (sort?.ToLowerInvariant(), IsAscending(direction)) switch
        {
            ("role", true) => query.OrderBy(user => user.Role).ThenBy(user => user.Name).ToList(),
            ("role", false) => query.OrderByDescending(user => user.Role).ThenByDescending(user => user.Name).ToList(),
            _ => query.ToList()
        };
    }

    public static IQueryable<Category> ApplyCategorySort(IQueryable<Category> query, string? sort, string? direction) =>
        (sort?.ToLowerInvariant(), IsAscending(direction)) switch
        {
            ("subcategories", true) => query.OrderBy(category => category.SubCategories.Count).ThenBy(category => category.Title),
            ("subcategories", false) => query.OrderByDescending(category => category.SubCategories.Count).ThenByDescending(category => category.Title),
            ("title", false) => query.OrderByDescending(category => category.Title),
            _ => query.OrderBy(category => category.Title)
        };

    public static IQueryable<SubCategory> ApplySubCategorySort(IQueryable<SubCategory> query, string? sort, string? direction) =>
        (sort?.ToLowerInvariant(), IsAscending(direction)) switch
        {
            ("category", true) => query.OrderBy(subCategory => subCategory.Category!.Title).ThenBy(subCategory => subCategory.Title),
            ("category", false) => query.OrderByDescending(subCategory => subCategory.Category!.Title).ThenByDescending(subCategory => subCategory.Title),
            ("title", false) => query.OrderByDescending(subCategory => subCategory.Title),
            _ => query.OrderBy(subCategory => subCategory.Title)
        };

    public static IQueryable<AttributeDefinition> ApplyAttributeSort(IQueryable<AttributeDefinition> query, string? sort, string? direction) =>
        (sort?.ToLowerInvariant(), IsAscending(direction)) switch
        {
            ("id", true) => query.OrderBy(attribute => attribute.Id),
            ("id", false) => query.OrderByDescending(attribute => attribute.Id),
            ("name", false) => query.OrderByDescending(attribute => attribute.Name),
            _ => query.OrderBy(attribute => attribute.Name)
        };

    public static IQueryable<Seller> ApplySellerSort(IQueryable<Seller> query, string? sort, string? direction) =>
        (sort?.ToLowerInvariant(), IsAscending(direction)) switch
        {
            ("company", true) => query.OrderBy(seller => seller.FantasyName ?? seller.Company).ThenBy(seller => seller.User!.Name),
            ("company", false) => query.OrderByDescending(seller => seller.FantasyName ?? seller.Company).ThenByDescending(seller => seller.User!.Name),
            ("email", true) => query.OrderBy(seller => seller.Email).ThenBy(seller => seller.User!.Name),
            ("email", false) => query.OrderByDescending(seller => seller.Email).ThenByDescending(seller => seller.User!.Name),
            ("cnpj", true) => query.OrderBy(seller => seller.Cnpj).ThenBy(seller => seller.User!.Name),
            ("cnpj", false) => query.OrderByDescending(seller => seller.Cnpj).ThenByDescending(seller => seller.User!.Name),
            ("name", false) => query.OrderByDescending(seller => seller.User!.Name).ThenByDescending(seller => seller.User!.LastName),
            _ => query.OrderBy(seller => seller.User!.Name).ThenBy(seller => seller.User!.LastName)
        };

    public static IQueryable<CarouselImage> ApplyCarouselSort(IQueryable<CarouselImage> query, string? sort, string? direction) =>
        (sort?.ToLowerInvariant(), IsAscending(direction)) switch
        {
            ("file", true) => query.OrderBy(image => image.FileName),
            ("file", false) => query.OrderByDescending(image => image.FileName),
            ("order", false) => query.OrderByDescending(image => image.SortOrder),
            _ => query.OrderBy(image => image.SortOrder)
        };

    private static bool IsAscending(string? direction) =>
        string.Equals(direction, "asc", StringComparison.OrdinalIgnoreCase);
}
