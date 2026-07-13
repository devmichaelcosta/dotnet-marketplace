using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Produto.ProductImports.GetById;

public sealed class GetProductImportJobByIdHandler(MarketplaceDbContext db)
{
    public async Task<ProductImportJobDetails?> HandleAsync(int id, CancellationToken cancellationToken)
    {
        var job = await db.ProductImportJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return job is null ? null : ProductImportJobDetails.From(job);
    }
}

