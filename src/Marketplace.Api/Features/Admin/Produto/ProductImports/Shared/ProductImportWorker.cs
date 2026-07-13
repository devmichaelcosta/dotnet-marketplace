using Marketplace.Api.Domain;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Produto.ProductImports.Shared;

public sealed class ProductImportWorker(
    IServiceScopeFactory scopeFactory,
    ProductImportQueue queue,
    ILogger<ProductImportWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RequeuePendingJobsAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var jobId = await queue.DequeueAsync(stoppingToken);
            try
            {
                using var scope = scopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<ProductImportProcessor>();
                await processor.ProcessAsync(jobId, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected failure while processing product import job {JobId}.", jobId);
            }
        }
    }

    private async Task RequeuePendingJobsAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MarketplaceDbContext>();
        var jobs = await db.ProductImportJobs
            .Where(job => job.Status == ProductImportJobStatus.Pending || job.Status == ProductImportJobStatus.Processing)
            .ToListAsync(cancellationToken);

        foreach (var job in jobs)
        {
            if (job.Status == ProductImportJobStatus.Processing)
            {
                job.Status = ProductImportJobStatus.Pending;
                job.StartedAt = null;
                job.SummaryMessage = "Retomado apos reinicio.";
            }

            await queue.EnqueueAsync(job.Id, cancellationToken);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}

