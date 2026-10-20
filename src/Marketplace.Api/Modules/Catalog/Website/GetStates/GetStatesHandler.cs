using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Modules.Catalog.Website.GetStates;

public sealed class GetStatesHandler(MarketplaceDbContext db)
{
    public async Task<IReadOnlyList<StateOption>> HandleAsync(CancellationToken cancellationToken)
    {
        return await db.States
            .AsNoTracking()
            .OrderBy(state => state.Name)
            .Select(state => new StateOption(state.Id, state.Name, state.Abbreviation))
            .ToListAsync(cancellationToken);
    }
}
