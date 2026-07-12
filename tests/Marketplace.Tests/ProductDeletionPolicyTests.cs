using Marketplace.Api.Domain;
using Marketplace.Api.Features.Products.Admin.Shared;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Tests;

public sealed class ProductDeletionPolicyTests
{
    [Fact]
    public async Task Product_deletion_is_blocked_when_order_items_exist()
    {
        await using var db = CreateContext();
        var policy = new ProductDeletionPolicy();

        try
        {
            await db.Database.EnsureCreatedAsync();

            var state = new State { Name = "State", Abbreviation = "ST" };
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "seller-user",
                Name = "Seller",
                LastName = "User",
                EmailConfirmed = true
            };
            db.States.Add(state);
            db.Users.Add(user);
            var product = new Product
            {
                UserId = user.Id,
                Title = "Produto",
                Description = "Descricao",
                Price = 10m,
                CreatedBy = "seed",
                Stock = 1,
                Sku = "SKU-001"
            };
            db.Products.Add(product);
            await db.SaveChangesAsync();

            db.Orders.Add(new Order
            {
                UserId = user.Id,
                StateId = state.Id,
                Total = 10m,
                Name = "Cliente",
                CardOwnerName = "Cliente",
                ExpirationDate = "12/30",
                Address = "Rua A",
                Neighborhood = "Centro",
                Cep = "00000-000",
                City = "Cidade",
                Cpf = "12345678901",
                Items = [new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = 1,
                    UnitPrice = 10m
                }]
            });
            await db.SaveChangesAsync();

            var validation = await policy.ValidateAsync(db, product.Id);

            Assert.NotNull(validation);
        }
        finally
        {
            await db.Database.EnsureDeletedAsync();
        }
    }

    [Fact]
    public async Task Product_deletion_is_allowed_when_order_items_do_not_exist()
    {
        await using var db = CreateContext();
        var policy = new ProductDeletionPolicy();

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
            var product = new Product
            {
                UserId = user.Id,
                Title = "Produto",
                Description = "Descricao",
                Price = 10m,
                CreatedBy = "seed",
                Stock = 1,
                Sku = "SKU-002"
            };
            db.Products.Add(product);
            await db.SaveChangesAsync();

            var validation = await policy.ValidateAsync(db, product.Id);

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
