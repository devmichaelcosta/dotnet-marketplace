using System.Text.Json;
using Marketplace.Tests.E2E.Pages;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Marketplace.Tests.E2E;

[Collection(MarketplaceE2eCollection.Name)]
public sealed class ProductToastThemeE2ETests(MarketplaceE2eFixture fixture)
{
    [Fact]
    public async Task Product_edit_success_notice_remains_readable_in_both_themes()
    {
        await using var session = await fixture.CreateSessionAsync();
        var page = session.Page;
        var editorPage = new AdminProductEditorPage(page);
        await SignInAsAdminAsync(page);
        await page.GotoAsync("/admin/products");
        await page.GetByTestId("admin-products-table").WaitForAsync();
        var toggleTheme = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Alternar tema" });

        foreach (var expectedTheme in new[] { "light", "dark" })
        {
            var currentTheme = await page.EvaluateAsync<string>("document.documentElement.dataset.theme");
            if (!string.Equals(currentTheme, expectedTheme, StringComparison.Ordinal))
            {
                await toggleTheme.ClickAsync();
            }

            await page.WaitForFunctionAsync("theme => document.documentElement.dataset.theme === theme", expectedTheme);
            await page.GotoAsync("/admin/products");
            await page.GetByTestId("admin-products-table").WaitForAsync();

            await page.GetByTestId("admin-products-row").First.GetByTestId("admin-products-edit").ClickAsync();
            await editorPage.WaitForReadyAsync();
            await editorPage.SaveAsync();
            await page.WaitForURLAsync("**/admin/products?saved=edited");

            var notice = page.GetByTestId("admin-products-notice");
            await Expect(notice).ToBeVisibleAsync();
            Assert.Equal("status", await notice.GetAttributeAsync("role"));
            Assert.Equal("polite", await notice.GetAttributeAsync("aria-live"));
            await Expect(notice.Locator("strong")).ToHaveCSSAsync("color", "rgb(23, 32, 51)");
            await Expect(notice.Locator("span")).ToHaveCSSAsync("color", "rgb(71, 84, 103)");
            var closeButton = notice.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Fechar notificacao" });
            await Expect(closeButton).ToBeVisibleAsync();
            await Expect(closeButton.Locator("i")).ToHaveCSSAsync("color", "rgb(23, 32, 51)");

            var closeButtonFocused = false;
            for (var tab = 0; tab < 100 && !closeButtonFocused; tab++)
            {
                await page.Keyboard.PressAsync("Tab");
                closeButtonFocused = await closeButton.EvaluateAsync<bool>("button => button === document.activeElement");
            }

            Assert.True(closeButtonFocused, "O controle de fechar deve ser alcancavel pela navegacao por teclado.");
            Assert.True(await closeButton.EvaluateAsync<bool>("button => button.matches(':focus-visible')"));
            await closeButton.ClickAsync();
            await Expect(notice).ToHaveCountAsync(0);
        }
    }

    private async Task SignInAsAdminAsync(IPage page)
    {
        var apiRequest = page.Context.APIRequest;
        var antiforgeryUrl = new Uri(new Uri(fixture.ApiBaseUrl), "auth/antiforgery").ToString();
        var antiforgeryResponse = await apiRequest.GetAsync(antiforgeryUrl);
        Assert.True(antiforgeryResponse.Ok, await antiforgeryResponse.TextAsync());
        using var payload = JsonDocument.Parse(await antiforgeryResponse.TextAsync());
        var token = payload.RootElement.GetProperty("token").GetString();
        Assert.False(string.IsNullOrWhiteSpace(token));

        var loginUrl = new Uri(new Uri(fixture.ApiBaseUrl), "auth/login").ToString();
        var loginResponse = await apiRequest.PostAsync(loginUrl, new APIRequestContextOptions
        {
            Data = JsonSerializer.Serialize(new { login = fixture.AdminUserName, password = fixture.AdminPassword }),
            Headers = new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["RequestVerificationToken"] = token!
            }
        });

        Assert.True(loginResponse.Ok, await loginResponse.TextAsync());
    }
}
