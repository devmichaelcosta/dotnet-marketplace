namespace Marketplace.Api.Features.Admin.Produto.ProductImports.Shared;

public sealed class ProductImportRow
{
    public int RowNumber { get; set; }

    public string Title { get; set; } = string.Empty;

    public string LoginVendedor { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Sku { get; set; } = string.Empty;

    public int Stock { get; set; }

    public bool Offer { get; set; }

    public string Category { get; set; } = string.Empty;

    public string SubCategory { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<string> ImageUrls { get; set; } = [];

    public List<string> DownloadedImages { get; } = [];

    public Dictionary<string, string> Attributes { get; } = new(StringComparer.OrdinalIgnoreCase);
}

