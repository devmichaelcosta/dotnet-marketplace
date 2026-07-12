using Marketplace.Tests.E2E.Pages;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Marketplace.Tests.E2E;

[Collection(MarketplaceE2eCollection.Name)]
public sealed class AdminE2ETests(MarketplaceE2eFixture fixture)
{
    [Fact]
    public async Task Anonymous_user_is_redirected_to_login_when_accessing_admin_users()
    {
        await using var session = await fixture.CreateSessionAsync();

        await session.Page.GotoAsync("/admin/users");

        var loginPage = new LoginPage(session.Page);
        Assert.True(await loginPage.IsVisibleAsync());
        Assert.Contains("/login", session.Page.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Admin_can_login_and_open_product_management()
    {
        await using var session = await fixture.CreateSessionAsync();
        var loginPage = new LoginPage(session.Page);
        var productsPage = new AdminProductsPage(session.Page);

        await loginPage.LoginAsync(fixture.AdminUserName, fixture.AdminPassword);
        await productsPage.NavigateAsync();

        await Expect(session.Page.GetByTestId("admin-products-table")).ToBeVisibleAsync();
        await Expect(session.Page.GetByTestId("admin-products-row").First).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Admin_can_create_product_end_to_end()
    {
        var uniqueSuffix = Guid.NewGuid().ToString("N")[..8];
        var title = $"Produto E2E {uniqueSuffix}";
        var sku = $"E2E-{uniqueSuffix}".ToUpperInvariant();

        var product = new AdminProductInput(
            Title: title,
            Description: "Produto criado por teste E2E para validar o fluxo administrativo.",
            Sku: sku,
            Price: "199.90",
            Stock: "7");

        await using var session = await fixture.CreateSessionAsync();
        var loginPage = new LoginPage(session.Page);
        var productsPage = new AdminProductsPage(session.Page);
        var editorPage = new AdminProductEditorPage(session.Page);

        await loginPage.LoginAsync(fixture.AdminUserName, fixture.AdminPassword);

        await productsPage.OpenCreateAsync();
        await editorPage.FillForCreateAsync(product);
        await editorPage.SaveAsync();
        await session.Page.WaitForURLAsync("**/admin/products?saved=created");
        await productsPage.WaitForSuccessNoticeAsync();
        await productsPage.SearchAsync(sku);
        await productsPage.WaitForProductAsync(sku);

        var productId = await fixture.WaitForProductIdBySkuAsync(sku);
        await session.Page.GotoAsync($"/admin/products/{productId}/edit");
        await editorPage.WaitForReadyAsync();
        await Expect(session.Page.GetByTestId("admin-product-title")).ToHaveValueAsync(title);
        await Expect(session.Page.GetByTestId("admin-product-sku")).ToHaveValueAsync(sku);
    }
}
