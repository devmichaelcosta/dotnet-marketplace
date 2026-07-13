using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Hosting;
using SkiaSharp;

namespace Marketplace.Api.Features.Admin.Produto.ProductImports.Shared;

public sealed class ProductImportImageDownloader(IHttpClientFactory httpClientFactory, IWebHostEnvironment environment)
{
    private const long MaxImageSizeBytes = 8 * 1024 * 1024;

    public async Task<ProductImportDownloadedImages> DownloadImagesAsync(
        string sku,
        IReadOnlyList<string> imageUrls,
        CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient("product-import-images");
        var relativeDirectory = Path.Combine("uploads", "products");
        var absoluteDirectory = Path.Combine(environment.WebRootPath, relativeDirectory);
        Directory.CreateDirectory(absoluteDirectory);

        var result = new ProductImportDownloadedImages();
        for (var index = 0; index < imageUrls.Count; index++)
        {
            var url = imageUrls[index];
            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new ProductImportException($"Nao foi possivel baixar a imagem {url}.");
            }

            await using var source = await response.Content.ReadAsStreamAsync(cancellationToken);
            await using var limited = new MemoryStream();
            await CopyWithLimitAsync(source, limited, MaxImageSizeBytes, cancellationToken);
            limited.Position = 0;

            using var bitmap = SKBitmap.Decode(limited);
            if (bitmap is null)
            {
                throw new ProductImportException($"URL nao retornou uma imagem valida: {url}.");
            }

            using var image = SKImage.FromBitmap(bitmap);
            using var encoded = image.Encode(SKEncodedImageFormat.Jpeg, 88);
            var fileName = $"{Slug(sku)}-{index + 1}-{Hash(url)}.jpg";
            var absolutePath = Path.Combine(absoluteDirectory, fileName);
            await using (var output = File.Create(absolutePath))
            {
                encoded.SaveTo(output);
            }

            result.AbsolutePaths.Add(absolutePath);
            result.RelativePaths.Add(fileName);
        }

        return result;
    }

    public static string Slug(string value)
    {
        var builder = new StringBuilder();
        foreach (var character in value.ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(character);
            }
            else if (builder.Length == 0 || builder[^1] != '-')
            {
                builder.Append('-');
            }
        }

        return builder.ToString().Trim('-');
    }

    private static async Task CopyWithLimitAsync(
        Stream source,
        Stream destination,
        long limit,
        CancellationToken cancellationToken)
    {
        var buffer = new byte[81920];
        long total = 0;
        int read;
        while ((read = await source.ReadAsync(buffer, cancellationToken)) > 0)
        {
            total += read;
            if (total > limit)
            {
                throw new ProductImportException("Imagem excede o limite de 8 MB.");
            }

            await destination.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
        }
    }

    private static string Hash(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant()[..8];
}

