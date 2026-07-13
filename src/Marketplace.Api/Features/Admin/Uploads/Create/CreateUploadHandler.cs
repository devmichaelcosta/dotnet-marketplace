using Marketplace.Api.Features.Admin.Shared;

namespace Marketplace.Api.Features.Admin.Uploads.Create;

public sealed class CreateUploadHandler(IWebHostEnvironment environment)
{
    public async Task<IResult> HandleAsync(string scope, IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]> { ["file"] = ["Arquivo obrigatorio."] });
        }

        if (file.Length > UploadPolicy.MaxSizeBytes)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]> { ["file"] = ["Imagem deve ter no maximo 5 MB."] });
        }

        if (!UploadPolicy.IsSupportedImage(file.FileName, file.ContentType))
        {
            return Results.ValidationProblem(new Dictionary<string, string[]> { ["file"] = ["Formato de imagem invalido."] });
        }

        scope = UploadPolicy.NormalizeScope(scope);
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var relativeDirectory = Path.Combine("uploads", scope);
        var absoluteDirectory = Path.Combine(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"), relativeDirectory);
        Directory.CreateDirectory(absoluteDirectory);

        var absolutePath = Path.Combine(absoluteDirectory, fileName);
        await using var stream = File.Create(absolutePath);
        await file.CopyToAsync(stream, cancellationToken);

        var publicPath = $"/uploads/{scope}/{fileName}";
        return Results.Ok(new UploadResponse(fileName, publicPath));
    }
}
