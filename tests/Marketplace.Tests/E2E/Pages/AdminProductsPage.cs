using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Marketplace.Tests.E2E.Pages;

public sealed class AdminProductsPage(IPage page)
{
    public async Task NavigateAsync()
    {
        await page.GotoAsync("/admin/products");
        await page.GetByTestId("admin-products-table").WaitForAsync();
    }

    public async Task OpenCreateAsync()
    {
        await page.GotoAsync("/admin/products/create");
    }

    public async Task SearchAsync(string search)
    {
        await FillAndCommitAsync(page.GetByTestId("admin-products-search"), search);
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Buscar" }).ClickAsync();
        await page.GetByTestId("admin-products-table").WaitForAsync();
    }

    public async Task OpenEditForAsync(string text)
    {
        var row = page.GetByTestId("admin-products-row").Filter(new LocatorFilterOptions { HasText = text });
        await row.GetByTestId("admin-products-edit").ClickAsync();
    }

    public async Task DeleteAsync(string text)
    {
        var row = page.GetByTestId("admin-products-row").Filter(new LocatorFilterOptions { HasText = text });

        void AcceptDialog(object? _, IDialog dialog)
        {
            _ = dialog.AcceptAsync();
        }

        page.Dialog += AcceptDialog;
        try
        {
            await row.GetByTestId("admin-products-delete").ClickAsync();
        }
        finally
        {
            page.Dialog -= AcceptDialog;
        }
    }

    public async Task WaitForProductAsync(string text)
    {
        await page.GetByTestId("admin-products-row").Filter(new LocatorFilterOptions { HasText = text }).WaitForAsync();
    }

    public async Task WaitForProductRemovalAsync(string text)
    {
        await Expect(page.GetByTestId("admin-products-row").Filter(new LocatorFilterOptions { HasText = text })).ToHaveCountAsync(0);
    }

    public async Task WaitForSuccessNoticeAsync()
    {
        await page.GetByTestId("admin-products-notice").WaitForAsync();
    }

    private static async Task FillAndCommitAsync(ILocator locator, string value)
    {
        await locator.ClickAsync();
        await locator.PressAsync("Control+A");
        await locator.PressSequentiallyAsync(value);
        await locator.PressAsync("Tab");
    }
}
