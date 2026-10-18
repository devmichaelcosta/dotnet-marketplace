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
        var testImage = "<svg xmlns='http://www.w3.org/2000/svg' width='1' height='1'></svg>";

        await page.RouteAsync("**/uploads/products/**", route => route.FulfillAsync(new RouteFulfillOptions
        {
            Status = 200,
            ContentType = "image/svg+xml",
            Body = testImage
        }));

        await page.GotoAsync("/");
        var offerImage = page.Locator(".market-product-card img").First;
        await Expect(offerImage).ToBeVisibleAsync();
        await offerImage.ScrollIntoViewIfNeededAsync();
        Assert.True(await offerImage.EvaluateAsync<bool>("image => image.decode().then(() => image.naturalWidth > 0)"));
        var offerImageUrl = await offerImage.GetAttributeAsync("src");
        Assert.Contains("/uploads/products/", offerImageUrl, StringComparison.OrdinalIgnoreCase);

        await page.GotoAsync("/?s=Ibanez");
        var searchImage = page.Locator(".market-product-card img").First;
        await Expect(searchImage).ToBeVisibleAsync();
        await searchImage.ScrollIntoViewIfNeededAsync();
        Assert.True(await searchImage.EvaluateAsync<bool>("image => image.decode().then(() => image.naturalWidth > 0)"));
        Assert.Contains("/uploads/products/", await searchImage.GetAttributeAsync("src"), StringComparison.OrdinalIgnoreCase);

        await searchImage.Locator("xpath=ancestor::a[contains(@class, 'market-product-media')]").ClickAsync();
        var detailImage = page.Locator(".legacy-product-main-image img").First;
        await Expect(detailImage).ToBeVisibleAsync();
        Assert.True(await detailImage.EvaluateAsync<bool>("image => image.decode().then(() => image.naturalWidth > 0)"));

        await page.RouteAsync("**/uploads/products/**", route => route.AbortAsync());
        await page.GotoAsync("/");
        await Expect(page.GetByTestId("product-image-fallback").First).ToBeVisibleAsync();
    }
}
