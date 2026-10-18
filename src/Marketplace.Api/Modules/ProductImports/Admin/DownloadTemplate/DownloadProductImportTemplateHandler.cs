using Microsoft.AspNetCore.Http.HttpResults;
using Marketplace.Api.Modules.ProductImports.Admin.Shared;

namespace Marketplace.Api.Modules.ProductImports.Admin.DownloadTemplate;

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

