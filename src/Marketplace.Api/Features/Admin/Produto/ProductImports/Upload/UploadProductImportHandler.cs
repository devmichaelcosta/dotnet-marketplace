using System.Globalization;
using Marketplace.Api.Domain;
using Marketplace.Api.Features;
using Marketplace.Api.Features.Admin.Produto.ProductImports.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Produto.ProductImports.Upload;

public sealed class UploadProductImportHandler(
    MarketplaceDbContext db,
    IWebHostEnvironment environment,
    ProductImportQueue queue,
    UploadProductImportValidator validator)
{
    public async Task<Results<Accepted<UploadProductImportCreatedResponse>, ValidationProblem, UnauthorizedHttpResult>> HandleAsync(
        IFormFile file,
        HttpContext http,
        CancellationToken cancellationToken)
    {
        var userId = http.User.GetUserId();
        if (userId is null)
        {
            return TypedResults.Unauthorized();
        }

        var errors = validator.Validate(file);
        if (errors.Count > 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        var job = new ProductImportJob
        {
            OriginalFileName = Path.GetFileName(file.FileName),
            StoredFileName = "original.xlsx",
            ContentType = file.ContentType,
            FileSizeBytes = file.Length,
            ImportedByUserId = userId.Value,
            ImportedByName = http.User.Identity?.Name ?? "admin",
            Status = ProductImportJobStatus.Pending,
            SummaryMessage = "Importacao aguardando processamento."
        };

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        db.ProductImportJobs.Add(job);
        await db.SaveChangesAsync(cancellationToken);

        var relativeDirectory = Path.Combine(
            "uploads",
            "product-imports",
            DateTimeOffset.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
            job.Id.ToString(CultureInfo.InvariantCulture));
        var absoluteDirectory = Path.Combine(environment.WebRootPath, relativeDirectory);
        Directory.CreateDirectory(absoluteDirectory);

        var absolutePath = Path.Combine(absoluteDirectory, job.StoredFileName);
        await using (var stream = File.Create(absolutePath))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        job.StoredFilePath = "/" + Path.Combine(relativeDirectory, job.StoredFileName).Replace('\\', '/');
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await queue.EnqueueAsync(job.Id, cancellationToken);
        return TypedResults.Accepted(
            $"/api/admin/product-imports/{job.Id}",
            new UploadProductImportCreatedResponse(job.Id));
    }
}

