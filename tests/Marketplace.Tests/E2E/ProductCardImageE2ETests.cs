using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Marketplace.Tests.E2E;

[Collection(MarketplaceE2eCollection.Name)]
public sealed class ProductCardImageE2ETests(MarketplaceE2eFixture fixture)
{
    [Fact]
    public async Task Product_images_load_in_home_offers_and_search_results()
    {
        await using var session = await fixture.CreateSessionAsync();
        var page = session.Page;
        var testImagePath = Path.Combine(fixture.SolutionRoot, "legado", "src", "assets", "img", "products", "Ibanez-RG-7420Z.jpg");

        var interceptedImageRequests = 0;
        await page.RouteAsync("**/*", async route =>
        {
            if (route.Request.Url.Contains("/uploads/products/", StringComparison.OrdinalIgnoreCase))
            {
                Interlocked.Increment(ref interceptedImageRequests);
                await route.FulfillAsync(new RouteFulfillOptions { Path = testImagePath });
                return;
            }

            await route.ContinueAsync();
        });

        await page.GotoAsync("/");
        await page.EvaluateAsync<object?>("() => document.querySelectorAll('.market-product-card img').forEach(image => image.loading = 'eager')");
        await page.WaitForFunctionAsync("() => { const image = document.querySelector('.market-product-card img'); return image && image.complete; }");
        var offerImage = page.Locator(".market-product-card img").First;
        await Expect(offerImage).ToBeVisibleAsync();
        Assert.True(interceptedImageRequests > 0, "A URL de imagem do card deve ser carregada pelo navegador.");
        Assert.True(await offerImage.EvaluateAsync<bool>("image => image.complete && image.naturalWidth > 0"));
        var offerImageUrl = await offerImage.GetAttributeAsync("src");
        Assert.Contains("/uploads/products/", offerImageUrl, StringComparison.OrdinalIgnoreCase);

        await page.GotoAsync("/?s=Ibanez");
        await page.EvaluateAsync<object?>("() => document.querySelectorAll('.market-product-card img').forEach(image => image.loading = 'eager')");
        await page.WaitForFunctionAsync("() => { const image = document.querySelector('.market-product-card img'); return image && image.complete; }");
        var searchImage = page.Locator(".market-product-card img").First;
        await Expect(searchImage).ToBeVisibleAsync();
        Assert.True(await searchImage.EvaluateAsync<bool>("image => image.complete && image.naturalWidth > 0"));
        Assert.Contains("/uploads/products/", await searchImage.GetAttributeAsync("src"), StringComparison.OrdinalIgnoreCase);

        await searchImage.Locator("xpath=ancestor::a[contains(@class, 'market-product-media')]").ClickAsync();
        var detailImage = page.Locator(".legacy-product-main-image img").First;
        await Expect(detailImage).ToBeVisibleAsync();
        await page.WaitForFunctionAsync("() => { const image = document.querySelector('.legacy-product-main-image img'); return image && image.complete; }");
        Assert.True(await detailImage.EvaluateAsync<bool>("image => image.complete && image.naturalWidth > 0"));

        await page.UnrouteAsync("**/*");
        await page.RouteAsync("**/uploads/products/**", route => route.AbortAsync());
        await page.GotoAsync("/");
        await Expect(page.GetByTestId("product-image-fallback").First).ToBeVisibleAsync();
    }
}
