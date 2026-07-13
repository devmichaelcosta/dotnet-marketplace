using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Orders.Search;

public sealed class SearchAdminOrdersHandler(MarketplaceDbContext db)
{
    public async Task<IReadOnlyList<AdminOrderSummaryResponse>> HandleAsync(SearchAdminOrdersQuery query, CancellationToken cancellationToken)
    {
        var orders = db.Orders.Include(order => order.User).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            orders = orders.Where(order =>
                order.Name.Contains(query.Search) ||
                order.City.Contains(query.Search) ||
                order.User!.Name.Contains(query.Search) ||
                order.User.UserName!.Contains(query.Search));
        }

        orders = (query.Sort?.ToLowerInvariant(), query.Direction?.Equals("asc", StringComparison.OrdinalIgnoreCase) == true) switch
        {
            ("id", true) => orders.OrderBy(order => order.Id),
            ("id", false) => orders.OrderByDescending(order => order.Id),
            ("customer", true) => orders.OrderBy(order => order.User!.Name).ThenBy(order => order.User!.UserName),
            ("customer", false) => orders.OrderByDescending(order => order.User!.Name).ThenByDescending(order => order.User!.UserName),
            ("city", true) => orders.OrderBy(order => order.City),
            ("city", false) => orders.OrderByDescending(order => order.City),
            ("total", true) => orders.OrderBy(order => order.Total),
            ("total", false) => orders.OrderByDescending(order => order.Total),
            ("created", true) => orders.OrderBy(order => order.CreatedAt),
            _ => orders.OrderByDescending(order => order.CreatedAt)
        };

        return await orders
            .Take(100)
            .Select(order => new AdminOrderSummaryResponse(
                order.Id,
                order.Total,
                order.CreatedAt,
                order.Name,
                order.City,
                order.User!.Name,
                order.User.UserName ?? string.Empty))
            .ToListAsync(cancellationToken);
    }
}
