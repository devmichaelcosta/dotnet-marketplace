using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Website.Catalog.GetStates;

public sealed class GetStatesHandler(MarketplaceDbContext db)
{
    public async Task<IReadOnlyList<StateOption>> HandleAsync(CancellationToken cancellationToken)
    {
        return await db.States
            .OrderBy(state => state.Name)
            .Select(state => new StateOption(state.Id, state.Name, state.Abbreviation))
            .ToListAsync(cancellationToken);
    }
}
