using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Features.Admin.Users.Search;

public sealed class SearchUsersHandler(UserManager<ApplicationUser> userManager)
{
    public async Task<List<UserResponse>> HandleAsync(SearchUsersQuery query, CancellationToken cancellationToken)
    {
        var usersQuery = userManager.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            usersQuery = usersQuery.Where(user =>
                user.Name.Contains(query.Search) ||
                user.LastName.Contains(query.Search) ||
                user.UserName!.Contains(query.Search) ||
                (user.Cpf != null && user.Cpf.Contains(query.Search)));
        }

        usersQuery = AdminListQueryPolicy.ApplyUserSort(usersQuery, query.Sort, query.Direction);
        var users = await usersQuery.ToListAsync(cancellationToken);
        var response = new List<UserResponse>();
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            response.Add(UserResponse.From(user, roles.ToArray()));
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            response = response
                .Where(user =>
                    $"{user.Name} {user.LastName}".Contains(query.Search, StringComparison.OrdinalIgnoreCase) ||
                    user.Login.Contains(query.Search, StringComparison.OrdinalIgnoreCase) ||
                    user.Role.Contains(query.Search, StringComparison.OrdinalIgnoreCase) ||
                    (user.Cpf?.Contains(query.Search, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();
        }

        return AdminListQueryPolicy.ApplyUserResponseSort(response, query.Sort, query.Direction);
    }
}
