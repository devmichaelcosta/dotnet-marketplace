using Marketplace.Api.Domain;
using Marketplace.Api.Features.Admin;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Tests;

public sealed class SellerDeletionPolicyTests
{
    [Fact]
    public async Task Seller_deletion_is_blocked_when_products_are_still_linked()
    {
        var databaseName = $"DotNetMarketplace_Test_{Guid.NewGuid():N}";
        var connectionString = $"Server=(localdb)\\SGPLocalDB;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
        var options = new DbContextOptionsBuilder<MarketplaceDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        await using var db = new MarketplaceDbContext(options);

        try
        {
            await db.Database.EnsureCreatedAsync();

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "seller-user",
                Name = "Seller",
                LastName = "User",
                EmailConfirmed = true
            };

            db.Users.Add(user);
            db.Products.Add(new Product
            {
                UserId = user.Id,
                Title = "Produto vinculado",
                Description = "Descricao",
                Price = 10m,
                CreatedBy = "seed",
                Stock = 1,
                Sku = "SKU-SELLER-001"
            });
            await db.SaveChangesAsync();

            var validation = await SellerDeletionPolicy.ValidateAsync(db, user.Id);

            Assert.NotNull(validation);
        }
        finally
        {
            await db.Database.EnsureDeletedAsync();
        }
    }

    [Fact]
    public async Task Seller_deletion_is_allowed_when_no_products_exist()
    {
        var databaseName = $"DotNetMarketplace_Test_{Guid.NewGuid():N}";
        var connectionString = $"Server=(localdb)\\SGPLocalDB;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
        var options = new DbContextOptionsBuilder<MarketplaceDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        await using var db = new MarketplaceDbContext(options);

        try
        {
            await db.Database.EnsureCreatedAsync();

            var userId = Guid.NewGuid();

            var validation = await SellerDeletionPolicy.ValidateAsync(db, userId);

            Assert.Null(validation);
        }
        finally
        {
            await db.Database.EnsureDeletedAsync();
        }
    }
}
