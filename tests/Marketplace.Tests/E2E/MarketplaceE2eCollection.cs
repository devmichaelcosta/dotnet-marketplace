using Microsoft.Playwright;
using Xunit;

namespace Marketplace.Tests.E2E;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class MarketplaceE2eCollection : ICollectionFixture<MarketplaceE2eFixture>
{
    public const string Name = "Marketplace E2E";
}

public sealed class BrowserSession(IBrowserContext context, IPage page) : IAsyncDisposable
{
    public IBrowserContext Context { get; } = context;
    public IPage Page { get; } = page;

    public async ValueTask DisposeAsync()
    {
        await Context.CloseAsync();
    }
}
