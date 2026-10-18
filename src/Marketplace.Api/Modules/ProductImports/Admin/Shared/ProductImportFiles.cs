namespace Marketplace.Api.Modules.ProductImports.Admin.Shared;

public static class ProductImportFiles
{
    public static string ToAbsolutePath(string webRootPath, string storedFilePath)
    {
        var relative = storedFilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var absolute = Path.GetFullPath(Path.Combine(webRootPath, relative));
        var root = Path.GetFullPath(webRootPath);
        if (!absolute.StartsWith(root, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Caminho de arquivo invalido.");
        }

        return absolute;
    }
}

