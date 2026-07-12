using Microsoft.Playwright;

namespace Marketplace.Tests.E2E.Pages;

public sealed class LoginPage(IPage page)
{
    public async Task NavigateAsync()
    {
        await page.GotoAsync("/login");
        await page.GetByTestId("login-form").WaitForAsync();
    }

    public async Task LoginAsync(string userName, string password)
    {
        await NavigateAsync();
        await FillAndCommitAsync(page.GetByTestId("login-username"), userName);
        await FillAndCommitAsync(page.GetByTestId("login-password"), password);
        await page.GetByTestId("login-submit").ClickAsync();
        await page.WaitForFunctionAsync(
            "() => window.location.pathname !== '/login' || !!document.querySelector('[data-testid=\"login-error\"]')");

        if (page.Url.Contains("/login", StringComparison.OrdinalIgnoreCase))
        {
            var message = await page.GetByTestId("login-error").TextContentAsync();
            throw new InvalidOperationException($"Falha ao autenticar no fluxo web: {message}");
        }
    }

    public async Task<bool> IsVisibleAsync()
    {
        return await page.GetByTestId("login-form").IsVisibleAsync();
    }

    private static async Task FillAndCommitAsync(ILocator locator, string value)
    {
        await locator.ClickAsync();
        await locator.PressAsync("Control+A");
        await locator.PressSequentiallyAsync(value);
        await locator.PressAsync("Tab");
    }
}
