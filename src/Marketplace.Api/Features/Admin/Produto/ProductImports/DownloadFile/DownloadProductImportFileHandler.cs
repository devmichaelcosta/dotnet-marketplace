using Marketplace.Api.Features.Admin.Produto.ProductImports.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Produto.ProductImports.DownloadFile;

public sealed class DownloadProductImportFileHandler(
    MarketplaceDbContext db,
    IWebHostEnvironment environment)
{
    public async Task<Results<FileContentHttpResult, NotFound>> HandleAsync(int id, CancellationToken cancellationToken)
    {
        var job = await db.ProductImportJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (job is null)
        {
            return TypedResults.NotFound();
        }

        var absolutePath = ProductImportFiles.ToAbsolutePath(environment.WebRootPath, job.StoredFilePath);
        if (!File.Exists(absolutePath))
        {
            return TypedResults.NotFound();
        }

        var bytes = await File.ReadAllBytesAsync(absolutePath, cancellationToken);
        return TypedResults.File(bytes, job.ContentType, job.OriginalFileName);
    }
}

