using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Marketplace.Tests.E2E.Pages;

public sealed class AdminProductEditorPage(IPage page)
{
    public async Task WaitForReadyAsync()
    {
        await page.Locator("[data-testid='admin-product-form'][data-interactive-ready='true']").WaitForAsync();
    }

    public async Task FillForCreateAsync(AdminProductInput input)
    {
        await WaitForReadyAsync();

        await FillAndCommitAsync(page.GetByTestId("admin-product-title"), input.Title);
        await SelectCurrentOrFirstOptionAsync(page.GetByTestId("admin-product-seller"));
        await FillAndCommitAsync(page.GetByTestId("admin-product-price"), input.Price);
        await FillAndCommitAsync(page.GetByTestId("admin-product-sku"), input.Sku);
        await FillAndCommitAsync(page.GetByTestId("admin-product-stock"), input.Stock);
        await SelectCurrentOrFirstOptionAsync(page.GetByTestId("admin-product-category"));
        await page.GetByTestId("admin-product-subcategory").WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible
        });
        await SelectCurrentOrFirstOptionAsync(page.GetByTestId("admin-product-subcategory"));
        await FillAndCommitAsync(page.GetByTestId("admin-product-description"), input.Description);
    }

    public async Task UpdateStockAsync(string stock)
    {
        await FillAndCommitAsync(page.GetByTestId("admin-product-stock"), stock);
    }

    public async Task SaveAsync()
    {
        await page.GetByTestId("admin-product-save").ClickAsync();
    }

    private static async Task SelectCurrentOrFirstOptionAsync(ILocator select)
    {
        await select.WaitForAsync();
        var hasSelection = await select.EvaluateAsync<bool>("element => Boolean(element.value)");
        if (hasSelection)
        {
            return;
        }

        var value = await select.EvaluateAsync<string>(
            @"element => {
                const selectElement = element;
                const nextOption = Array.from(selectElement.options).find(option => option.value);
                return nextOption ? nextOption.value : '';
            }");

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException("Nenhuma opcao selecionavel foi encontrada.");
        }

        await select.SelectOptionAsync(new[] { value });
        await Expect(select).ToHaveAttributeAsync("data-field-modified", "true");
    }

    private static async Task FillAndCommitAsync(ILocator locator, string value)
    {
        await locator.ClickAsync();
        await locator.PressAsync("Control+A");
        await locator.PressSequentiallyAsync(value);
        await locator.PressAsync("Tab");
        await Expect(locator).ToHaveAttributeAsync("data-field-modified", "true");
    }
}
