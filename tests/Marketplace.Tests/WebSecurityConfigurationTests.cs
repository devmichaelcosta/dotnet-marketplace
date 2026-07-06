namespace Marketplace.Tests;

public sealed class WebSecurityConfigurationTests
{
    [Fact]
    public void Web_auth_endpoints_validate_antiforgery_tokens()
    {
        var program = ReadSource("src", "Marketplace.Web", "Program.cs");

        Assert.Contains("app.MapGet(\"/auth/antiforgery\"", program);
        Assert.Contains("IAntiforgery antiforgery", program);
        Assert.Contains("await antiforgery.ValidateRequestAsync(http)", program);
        Assert.DoesNotContain("DisableAntiforgery", program);
    }

    [Theory]
    [InlineData("AdminCarousel.razor", "admin")]
    [InlineData("AdminCatalog.razor", "admin")]
    [InlineData("AdminOrders.razor", "admin")]
    [InlineData("AdminProductImports.razor", "admin")]
    [InlineData("AdminRatings.razor", "admin")]
    [InlineData("AdminUsers.razor", "admin")]
    [InlineData("AdminProducts.razor", "admin,vendedor")]
    [InlineData("AdminProductsCreate.razor", "admin,vendedor")]
    [InlineData("AdminProductsEdit.razor", "admin,vendedor")]
    [InlineData("AdminSellers.razor", "admin,vendedor")]
    public void Admin_pages_are_protected_by_authorize_attributes(string fileName, string roles)
    {
        var page = ReadSource("src", "Marketplace.Web", "Components", "Pages", fileName);

        Assert.Contains($"@attribute [Authorize(Roles = \"{roles}\")]", page);
    }

    [Fact]
    public void Product_admin_uses_separate_create_and_edit_pages()
    {
        var create = ReadSource("src", "Marketplace.Web", "Components", "Pages", "AdminProductsCreate.razor");
        var edit = ReadSource("src", "Marketplace.Web", "Components", "Pages", "AdminProductsEdit.razor");
        var editor = ReadSource("src", "Marketplace.Web", "Components", "Shared", "AdminProductEditor.razor");
        var similar = ReadSource("src", "Marketplace.Web", "Components", "Pages", "AdminProductsSimilarProducts.razor");
        var listPage = ReadSource("src", "Marketplace.Web", "Components", "Pages", "AdminProducts.razor");

        Assert.Contains("@page \"/admin/products/create\"", create);
        Assert.Contains("@page \"/admin/products/{Id:int}/edit\"", edit);
        Assert.Contains("Informe os dados do produto e clique em salvar.", editor);
        Assert.Contains("Adicionar atributo", editor);
        Assert.Contains("/admin/products/{ProductId}/similar", editor);
        Assert.Contains("Produtos similares", editor);
        Assert.Contains("@page \"/admin/products/{Id:int}/similar\"", similar);
        Assert.Contains("admin-data-table", listPage);
        Assert.Contains("admin-table-scroll", listPage);
    }

    [Fact]
    public void Product_admin_list_places_actions_in_the_first_column()
    {
        var listPage = ReadSource("src", "Marketplace.Web", "Components", "Pages", "AdminProducts.razor");

        Assert.Contains("<th class=\"actions-column admin-product-actions\"></th>", listPage);
        Assert.Contains("<td class=\"actions-column admin-product-actions\">", listPage);
        Assert.DoesNotContain("table-actions", listPage);
    }

    [Fact]
    public void Category_admin_uses_separate_create_and_edit_pages()
    {
        var create = ReadSource("src", "Marketplace.Web", "Components", "Pages", "AdminCategoriesCreate.razor");
        var edit = ReadSource("src", "Marketplace.Web", "Components", "Pages", "AdminCategoriesEdit.razor");
        var editor = ReadSource("src", "Marketplace.Web", "Components", "Shared", "AdminCategoryEditor.razor");
        var listPage = ReadSource("src", "Marketplace.Web", "Components", "Pages", "AdminCatalog.razor");

        Assert.Contains("@page \"/admin/catalog/categories/create\"", create);
        Assert.Contains("@page \"/admin/catalog/categories/{Id:int}/edit\"", edit);
        Assert.Contains("Nova categoria", editor);
        Assert.Contains("Editar categoria", editor);
        Assert.Contains("admin-data-table", listPage);
        Assert.Contains("CreateCategory", listPage);
        Assert.Contains("EditCategory(category.Id)", listPage);
    }

    [Fact]
    public void SubCategory_admin_uses_separate_create_and_edit_pages()
    {
        var create = ReadSource("src", "Marketplace.Web", "Components", "Pages", "AdminSubCategoriesCreate.razor");
        var edit = ReadSource("src", "Marketplace.Web", "Components", "Pages", "AdminSubCategoriesEdit.razor");
        var editor = ReadSource("src", "Marketplace.Web", "Components", "Shared", "AdminSubCategoryEditor.razor");

        Assert.Contains("@page \"/admin/catalog/subcategories/create\"", create);
        Assert.Contains("@page \"/admin/catalog/subcategories/{Id:int}/edit\"", edit);
        Assert.Contains("Nova subcategoria", editor);
        Assert.Contains("Editar subcategoria", editor);
    }

    [Fact]
    public void Attribute_admin_uses_separate_create_and_edit_pages()
    {
        var create = ReadSource("src", "Marketplace.Web", "Components", "Pages", "AdminAttributesCreate.razor");
        var edit = ReadSource("src", "Marketplace.Web", "Components", "Pages", "AdminAttributesEdit.razor");
        var editor = ReadSource("src", "Marketplace.Web", "Components", "Shared", "AdminAttributeEditor.razor");

        Assert.Contains("@page \"/admin/catalog/attributes/create\"", create);
        Assert.Contains("@page \"/admin/catalog/attributes/{Id:int}/edit\"", edit);
        Assert.Contains("Novo atributo", editor);
        Assert.Contains("Editar atributo", editor);
    }

    [Fact]
    public void Orders_page_uses_the_same_breadcrumb_pattern_without_dark_box()
    {
        var page = ReadSource("src", "Marketplace.Web", "Components", "Pages", "Orders.razor");

        Assert.Contains("<nav class=\"nav-breadcrumb admin-breadcrumb\">", page);
        Assert.DoesNotContain("nav-breadcrumb-dark admin-breadcrumb", page);
    }

    [Theory]
    [InlineData("AdminUsers.razor")]
    [InlineData("AdminSellers.razor")]
    [InlineData("AdminRatings.razor")]
    [InlineData("AdminCarousel.razor")]
    public void Admin_list_pages_use_datatable_search_and_icon_actions(string fileName)
    {
        var page = ReadSource("src", "Marketplace.Web", "Components", "Pages", fileName);

        Assert.Contains("admin-data-table", page);
        Assert.Contains("@onsubmit", page);
        Assert.Contains("table-sort", page);
        Assert.Contains("icon-action", page);
    }

    [Fact]
    public void Admin_orders_page_uses_datatable_search_and_sort()
    {
        var page = ReadSource("src", "Marketplace.Web", "Components", "Pages", "AdminOrders.razor");

        Assert.Contains("admin-data-table", page);
        Assert.Contains("@onsubmit", page);
        Assert.Contains("table-sort", page);
    }

    [Theory]
    [InlineData("LikedProducts.razor")]
    [InlineData("Orders.razor")]
    [InlineData("Profile.razor")]
    public void Authenticated_user_pages_are_protected_by_authorize_attributes(string fileName)
    {
        var page = ReadSource("src", "Marketplace.Web", "Components", "Pages", fileName);

        Assert.Contains("@attribute [Authorize]", page);
    }

    [Fact]
    public void Legacy_parity_matrix_tracks_all_major_modules()
    {
        var matrix = ReadSource("docs", "legacy-parity-matrix.md");
        var modules = new[]
        {
            "Home/catalogo",
            "Detalhe de produto",
            "Login/logout/registro",
            "Usuarios admin",
            "Vendedores",
            "Produtos admin",
            "Carrinho",
            "Checkout",
            "Pedidos",
            "Diferente por seguranca"
        };

        foreach (var module in modules)
        {
            Assert.Contains(module, matrix);
        }
    }

    private static string ReadSource(params string[] relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "DotNetMarketplace.slnx")))
        {
            directory = directory.Parent;
        }

        Assert.NotNull(directory);
        return File.ReadAllText(Path.Combine([directory!.FullName, .. relativePath]));
    }
}
