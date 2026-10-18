using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Marketplace.Tests.E2E;

[Collection(MarketplaceE2eCollection.Name)]
public sealed class PasswordRecoveryE2ETests(MarketplaceE2eFixture fixture)
{
    [Fact]
    public async Task Request_shows_same_response_for_existing_and_unknown_logins()
    {
        await using var session = await fixture.CreateSessionAsync();
        await session.Page.GotoAsync("/recover-password");
        await session.Page.Locator("[data-testid='recover-password-form'][data-interactive-ready='true']").WaitForAsync();

        var loginInput = session.Page.GetByTestId("recover-password-login");
        var submitButton = session.Page.GetByTestId("recover-password-submit");
        var response = session.Page.GetByTestId("recover-password-message");

        await loginInput.FillAsync(fixture.AdminUserName);
        await loginInput.PressAsync("Tab");
        await submitButton.ClickAsync();
        await Expect(response).ToHaveTextAsync(
            "Se a conta estiver cadastrada e puder receber mensagens, enviaremos as instrucoes para o e-mail associado.");

        await loginInput.FillAsync("account-that-does-not-exist");
        await loginInput.PressAsync("Tab");
        await submitButton.ClickAsync();
        await Expect(response).ToHaveTextAsync(
            "Se a conta estiver cadastrada e puder receber mensagens, enviaremos as instrucoes para o e-mail associado.");
    }
}
