namespace Marketplace.Api.Features.Admin.Produto.ProductImports.Upload;

public sealed class UploadProductImportValidator
{
    public const long MaxExcelSizeBytes = 5 * 1024 * 1024;

    public Dictionary<string, string[]> Validate(IFormFile file)
    {
        var errors = new Dictionary<string, string[]>();
        if (file.Length <= 0)
        {
            errors["file"] = ["Arquivo obrigatorio."];
        }
        else if (file.Length > MaxExcelSizeBytes)
        {
            errors["file"] = ["Arquivo deve ter no maximo 5 MB."];
        }

        var extension = Path.GetExtension(file.FileName);
        if (!extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase) &&
            !extension.Equals(".xls", StringComparison.OrdinalIgnoreCase))
        {
            errors["fileType"] = ["Envie uma planilha Excel .xlsx ou .xls."];
        }

        return errors;
    }
}

