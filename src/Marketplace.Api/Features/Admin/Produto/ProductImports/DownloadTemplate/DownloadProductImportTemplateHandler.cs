using Microsoft.AspNetCore.Http.HttpResults;
using Marketplace.Api.Features.Admin.Produto.ProductImports.Shared;

namespace Marketplace.Api.Features.Admin.Produto.ProductImports.DownloadTemplate;

public sealed class DownloadProductImportTemplateHandler
{
    public Task<FileContentHttpResult> HandleAsync()
    {
        var bytes = ProductImportTemplate.Create();
        var result = TypedResults.File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "product-import-template.xlsx");

        return Task.FromResult(result);
    }
}

