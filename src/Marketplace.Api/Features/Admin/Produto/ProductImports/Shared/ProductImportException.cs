namespace Marketplace.Api.Features.Admin.Produto.ProductImports.Shared;

public sealed class ProductImportException : Exception
{
    public ProductImportException(string message, int rowNumber = 0, string sku = "", string title = "") : base(message)
    {
        RowNumber = rowNumber;
        Sku = sku;
        Title = title;
    }

    public int RowNumber { get; }

    public string Sku { get; }

    public string Title { get; }
}

