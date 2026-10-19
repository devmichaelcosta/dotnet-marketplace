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
        var title = page.GetByTestId("admin-product-title");
        await FillAndCommitAsync(title, input.Title);
        await Expect(title).ToHaveAttributeAsync("data-bound-value", input.Title);
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
        if (!string.IsNullOrWhiteSpace(await select.InputValueAsync()))
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

        await select.FocusAsync();
        await select.PressAsync("ArrowDown");
        await Expect(select).ToHaveValueAsync(value);
    }

    private static async Task FillAndCommitAsync(ILocator locator, string value)
    {
        await locator.FillAsync(value);
        await locator.BlurAsync();
        if (string.Equals(await locator.GetAttributeAsync("type"), "number", StringComparison.OrdinalIgnoreCase))
        {
            await locator.DispatchEventAsync("change");
        }

        await Expect(locator).ToHaveAttributeAsync("data-field-modified", "true");
    }
}
