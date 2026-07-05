using Marketplace.Api.Domain;
using Marketplace.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Marketplace.Tests;

public sealed class InfrastructureModelTests
{
    [Fact]
    public void SqlServer_model_uses_no_action_on_relationships_that_can_create_multiple_cascade_paths()
    {
        using var context = CreateContext();

        AssertDeleteBehavior<Address, State>(context, "StateId", DeleteBehavior.NoAction);
        AssertDeleteBehavior<CartItem, Product>(context, "ProductId", DeleteBehavior.NoAction);
        AssertDeleteBehavior<Order, ApplicationUser>(context, "UserId", DeleteBehavior.NoAction);
        AssertDeleteBehavior<Order, State>(context, "StateId", DeleteBehavior.NoAction);
        AssertDeleteBehavior<OrderItem, Product>(context, "ProductId", DeleteBehavior.NoAction);
        AssertDeleteBehavior<Product, ApplicationUser>(context, "UserId", DeleteBehavior.NoAction);
    }

    [Fact]
    public void SqlServer_model_keeps_owned_child_data_cascading_from_aggregate_roots()
    {
        using var context = CreateContext();

        AssertDeleteBehavior<OrderItem, Order>(context, "OrderId", DeleteBehavior.Cascade);
        AssertDeleteBehavior<ProductImage, Product>(context, "ProductId", DeleteBehavior.Cascade);
        AssertDeleteBehavior<ProductAttributeValue, Product>(context, "ProductId", DeleteBehavior.Cascade);
    }

    private static MarketplaceDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<MarketplaceDbContext>()
            .UseSqlServer("Server=(localdb)\\SGPLocalDB;Database=DotNetMarketplace_ModelValidation;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;

        return new MarketplaceDbContext(options);
    }

    private static void AssertDeleteBehavior<TDependent, TPrincipal>(
        MarketplaceDbContext context,
        string foreignKeyProperty,
        DeleteBehavior expected)
    {
        var dependent = context.Model.FindEntityType(typeof(TDependent));
        Assert.NotNull(dependent);

        var foreignKey = dependent.GetForeignKeys().Single(key =>
            key.PrincipalEntityType.ClrType == typeof(TPrincipal) &&
            key.Properties.Any(property => property.Name == foreignKeyProperty));

        Assert.Equal(expected, foreignKey.DeleteBehavior);
    }
}
