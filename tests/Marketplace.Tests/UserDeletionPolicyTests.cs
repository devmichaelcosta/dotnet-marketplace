using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Tests;

public sealed class UserDeletionPolicyTests
{
    [Fact]
    public async Task User_deletion_is_blocked_when_transactional_links_exist()
    {
        await using var db = CreateContext();

        try
        {
            await db.Database.EnsureCreatedAsync();

            var state = new State { Name = "State", Abbreviation = "ST" };
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "customer-user",
                Name = "Customer",
                LastName = "User",
                EmailConfirmed = true
            };

            db.States.Add(state);
            db.Users.Add(user);
            db.Orders.Add(new Order
            {
                UserId = user.Id,
                State = state,
                Total = 10m,
                Name = "Cliente",
                CardOwnerName = "Cliente",
                ExpirationDate = "12/30",
                Address = "Rua A",
                Neighborhood = "Centro",
                Cep = "00000-000",
                City = "Cidade",
                Cpf = "12345678901"
            });
            await db.SaveChangesAsync();

            var validation = await UserDeletionPolicy.ValidateAsync(db, user.Id);

            Assert.NotNull(validation);
        }
        finally
        {
            await db.Database.EnsureDeletedAsync();
        }
    }

    [Fact]
    public async Task User_deletion_is_allowed_when_no_links_exist()
    {
        await using var db = CreateContext();

        try
        {
            await db.Database.EnsureCreatedAsync();

            var validation = await UserDeletionPolicy.ValidateAsync(db, Guid.NewGuid());

            Assert.Null(validation);
        }
        finally
        {
            await db.Database.EnsureDeletedAsync();
        }
    }

    private static MarketplaceDbContext CreateContext()
    {
        var databaseName = $"DotNetMarketplace_Test_{Guid.NewGuid():N}";
        var connectionString = $"Server=(localdb)\\SGPLocalDB;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
        var options = new DbContextOptionsBuilder<MarketplaceDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new MarketplaceDbContext(options);
    }
}
