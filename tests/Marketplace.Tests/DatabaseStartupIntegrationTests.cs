using Marketplace.Api.Domain;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Tests;

public sealed class DatabaseStartupIntegrationTests
{
    [Fact]
    public async Task Development_startup_can_create_database_apply_migrations_and_seed_initial_data()
    {
        var databaseName = $"DotNetMarketplace_Test_{Guid.NewGuid():N}";
        var connectionString = $"Server=(localdb)\\SGPLocalDB;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
        var services = CreateServices(connectionString);

        try
        {
            await MarketplaceSeed.SeedAsync(services);

            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MarketplaceDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            Assert.True(await db.Database.CanConnectAsync());
            Assert.True(await db.States.CountAsync() >= 2);
            Assert.True(await db.Categories.AnyAsync());
            Assert.True(await db.SubCategories.AnyAsync());
            Assert.True(await db.Products.CountAsync() >= 1000);
            Assert.True(await db.CarouselImages.AnyAsync());
            Assert.True(await db.ProductImages.AnyAsync());
            Assert.False(await db.ProductImages.AnyAsync(image => image.FileName.StartsWith("/uploads/products/")));
            Assert.True(await db.ProductAttributeValues.CountAsync() >= 1000);
            Assert.True(await db.SimilarProducts.CountAsync() >= 1000);
            Assert.True(await db.CarouselImages.AnyAsync(image => image.FileName.StartsWith("/uploads/carousel/")));
            Assert.True(await db.Addresses.AnyAsync());

            var admin = await userManager.FindByNameAsync("michael");
            Assert.NotNull(admin);
            Assert.True(await userManager.IsInRoleAsync(admin, MarketplaceSeed.AdminRole));
        }
        finally
        {
            await DeleteDatabaseAsync(connectionString);
            if (services is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    private static ServiceProvider CreateServices(string connectionString)
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddDbContext<MarketplaceDbContext>(options => options.UseSqlServer(connectionString));
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<MarketplaceDbContext>()
            .AddSignInManager();

        return services.BuildServiceProvider();
    }

    private static async Task DeleteDatabaseAsync(string connectionString)
    {
        var options = new DbContextOptionsBuilder<MarketplaceDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        await using var db = new MarketplaceDbContext(options);
        await db.Database.EnsureDeletedAsync();
    }
}
