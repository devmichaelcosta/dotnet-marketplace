namespace Marketplace.Tests.E2E.Pages;

public sealed record AdminProductInput(
    string Title,
    string Description,
    string Sku,
    string Price,
    string Stock);
