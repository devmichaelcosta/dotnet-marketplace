using Marketplace.Api.Domain;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public sealed record BenchmarkDataset(Guid AdminId, Guid SellerId, Guid[] UserIds, int StateId, int CategoryId, int SubCategoryId, int[] ProductIds);

public static class BenchmarkDatasetFactory
{
    public static async Task<BenchmarkDataset> CreateAsync(DbContextOptions<MarketplaceDbContext> options)
    {
        await using var db = new MarketplaceDbContext(options);
        var state = await db.States.OrderBy(item => item.Id).FirstOrDefaultAsync();
        if (state is null)
        {
            state = new State { Name = "Benchmark State", Abbreviation = "BM" };
            db.States.Add(state);
        }

        var category = new Category { Title = "Benchmark Catalog" };
        var subcategory = new SubCategory { Title = "Benchmark Items", Category = category };
        db.Categories.Add(category);
        db.SubCategories.Add(subcategory);
        await db.SaveChangesAsync();

        var users = Enumerable.Range(0, 2000).Select(index => new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "benchmark-user-" + index.ToString("D4"),
            NormalizedUserName = "BENCHMARK-USER-" + index.ToString("D4"),
            Email = "benchmark-user-" + index.ToString("D4") + "@example.invalid",
            NormalizedEmail = "BENCHMARK-USER-" + index.ToString("D4") + "@EXAMPLE.INVALID",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString("N"),
            ConcurrencyStamp = Guid.NewGuid().ToString("N"),
            Name = index == 0 ? "Benchmark Seller" : "Benchmark User " + index.ToString("D4"),
            LastName = "Synthetic"
        }).ToArray();
        db.Users.AddRange(users);
        db.Sellers.Add(new Seller
        {
            Id = Guid.NewGuid(),
            UserId = users[0].Id,
            User = users[0],
            Email = users[0].Email,
            Company = "Synthetic Benchmark Seller",
            FantasyName = "Benchmark Seller"
        });
        await db.SaveChangesAsync();

        var productIds = new List<int>(10_000);
        for (var start = 0; start < 10_000; start += 500)
        {
            var products = Enumerable.Range(start, Math.Min(500, 10_000 - start)).Select(index => new Product
            {
                UserId = users[index % users.Length].Id,
                SubCategoryId = subcategory.Id,
                Title = "Benchmark Product " + index.ToString("D5"),
                Description = "Deterministic synthetic catalog record used only for local performance baselines.",
                Price = 10m + index % 500,
                CreatedAt = DateTimeOffset.UnixEpoch.AddMinutes(index),
                CreatedBy = "benchmark-seed",
                Offer = index % 3 == 0,
                Stock = index == 0 ? 100 : 25 + index % 200,
                Sku = "BENCH-P-" + index.ToString("D5")
            }).ToArray();
            db.Products.AddRange(products);
            await db.SaveChangesAsync();
            productIds.AddRange(products.Select(item => item.Id));
        }

        for (var start = 0; start < 10_000; start += 200)
        {
            var orders = Enumerable.Range(start, Math.Min(200, 10_000 - start)).Select(index =>
            {
                var items = Enumerable.Range(0, 5).Select(offset =>
                {
                    var productIndex = (index * 5 + offset) % productIds.Count;
                    return new OrderItem
                    {
                        ProductId = productIds[productIndex],
                        Quantity = 1 + (index + offset) % 3,
                        UnitPrice = 10m + productIndex % 500
                    };
                }).ToList();
                return new Order
                {
                    UserId = users[(index + 1) % users.Length].Id,
                    StateId = state.Id,
                    CreatedAt = DateTimeOffset.UnixEpoch.AddMinutes(index),
                    Name = "Synthetic Customer " + index.ToString("D5"),
                    Address = "1 Benchmark Street",
                    Neighborhood = "Synthetic District",
                    Cep = "90000000",
                    City = "Benchmark City",
                    Cpf = "52998224725",
                    Total = items.Sum(item => item.Quantity * item.UnitPrice),
                    Items = items
                };
            }).ToArray();
            db.Orders.AddRange(orders);
            await db.SaveChangesAsync();
        }

        return new BenchmarkDataset(users[1].Id, users[0].Id, users.Select(item => item.Id).ToArray(), state.Id, category.Id, subcategory.Id, productIds.ToArray());
    }
}
