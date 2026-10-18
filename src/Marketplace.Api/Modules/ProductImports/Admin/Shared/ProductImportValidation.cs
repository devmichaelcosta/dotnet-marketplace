using Marketplace.Api.Domain;

namespace Marketplace.Api.Modules.ProductImports.Admin.Shared;

public sealed class ProductImportValidation
{
    public Dictionary<string, ApplicationUser> SellersByLogin { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, Category> CategoriesByName { get; } = new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<int, SubCategory> SubCategoriesByRow { get; } = [];

    public List<ProductImportJobItem> ErrorItems { get; } = [];
}

