using Marketplace.Api.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Infrastructure.Persistence;

public sealed class MarketplaceDbContext(DbContextOptions<MarketplaceDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Seller> Sellers => Set<Seller>();
    public DbSet<State> States => Set<State>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<SubCategory> SubCategories => Set<SubCategory>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<AttributeDefinition> Attributes => Set<AttributeDefinition>();
    public DbSet<ProductAttributeValue> ProductAttributeValues => Set<ProductAttributeValue>();
    public DbSet<SimilarProduct> SimilarProducts => Set<SimilarProduct>();
    public DbSet<ProductLike> ProductLikes => Set<ProductLike>();
    public DbSet<ProductRating> ProductRatings => Set<ProductRating>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<CarouselImage> CarouselImages => Set<CarouselImage>();
    public DbSet<ProductImportJob> ProductImportJobs => Set<ProductImportJob>();
    public DbSet<ProductImportJobItem> ProductImportJobItems => Set<ProductImportJobItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.Name).HasMaxLength(120).IsRequired();
            entity.Property(user => user.LastName).HasMaxLength(120);
            entity.Property(user => user.Cpf).HasMaxLength(20);
            entity.HasMany(user => user.Products)
                .WithOne(product => product.User)
                .HasForeignKey(product => product.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasMany(user => user.Orders)
                .WithOne(order => order.User)
                .HasForeignKey(order => order.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Seller>(entity =>
        {
            entity.HasKey(seller => seller.Id);
            entity.HasIndex(seller => seller.UserId).IsUnique();
            entity.Property(seller => seller.Email).HasMaxLength(180);
            entity.Property(seller => seller.Website).HasMaxLength(250);
            entity.Property(seller => seller.Company).HasMaxLength(180);
            entity.Property(seller => seller.Cnpj).HasMaxLength(32);
            entity.Property(seller => seller.FantasyName).HasMaxLength(180);
        });

        builder.Entity<State>(entity =>
        {
            entity.Property(state => state.Name).HasMaxLength(120).IsRequired();
            entity.Property(state => state.Abbreviation).HasMaxLength(2).IsRequired();
        });

        builder.Entity<Address>(entity =>
        {
            entity.Property(address => address.Street).HasMaxLength(250).IsRequired();
            entity.Property(address => address.Cep).HasMaxLength(16).IsRequired();
            entity.Property(address => address.Neighborhood).HasMaxLength(120).IsRequired();
            entity.Property(address => address.City).HasMaxLength(120).IsRequired();
            entity.Property(address => address.Complement).HasMaxLength(250);
            entity.HasOne(address => address.State)
                .WithMany()
                .HasForeignKey(address => address.StateId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Category>(entity =>
        {
            entity.Property(category => category.Title).HasMaxLength(120).IsRequired();
            entity.Property(category => category.Image).HasMaxLength(260);
        });

        builder.Entity<SubCategory>(entity =>
        {
            entity.Property(subCategory => subCategory.Title).HasMaxLength(120).IsRequired();
            entity.HasOne(subCategory => subCategory.Category)
                .WithMany(category => category.SubCategories)
                .HasForeignKey(subCategory => subCategory.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Product>(entity =>
        {
            entity.Property(product => product.Title).HasMaxLength(250).IsRequired();
            entity.Property(product => product.Description).IsRequired();
            entity.Property(product => product.Price).HasPrecision(18, 2);
            entity.Property(product => product.CreatedBy).HasMaxLength(120).IsRequired();
            entity.Property(product => product.Sku).HasMaxLength(80).IsRequired();
            entity.HasOne(product => product.SubCategory)
                .WithMany(subCategory => subCategory.Products)
                .HasForeignKey(product => product.SubCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<ProductImage>(entity =>
        {
            entity.Property(image => image.FileName).HasMaxLength(260).IsRequired();
        });

        builder.Entity<AttributeDefinition>(entity =>
        {
            entity.ToTable("AttributeDefinitions");
            entity.Property(attribute => attribute.Name).HasMaxLength(120).IsRequired();
        });

        builder.Entity<ProductAttributeValue>(entity =>
        {
            entity.Property(value => value.Value).HasMaxLength(500).IsRequired();
        });

        builder.Entity<SimilarProduct>(entity =>
        {
            entity.HasKey(similar => new { similar.ParentProductId, similar.ChildProductId });
            entity.HasOne(similar => similar.ParentProduct)
                .WithMany()
                .HasForeignKey(similar => similar.ParentProductId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(similar => similar.ChildProduct)
                .WithMany()
                .HasForeignKey(similar => similar.ChildProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ProductLike>(entity =>
        {
            entity.HasKey(like => new { like.UserId, like.ProductId });
        });

        builder.Entity<ProductRating>(entity =>
        {
            entity.Property(rating => rating.Title).HasMaxLength(160).IsRequired();
            entity.Property(rating => rating.Description).HasMaxLength(2000).IsRequired();
            entity.Property(rating => rating.Rating).HasMaxLength(40).IsRequired();
        });

        builder.Entity<Cart>(entity =>
        {
            entity.Property(cart => cart.AnonymousKey).HasMaxLength(64).IsRequired();
            entity.HasIndex(cart => cart.AnonymousKey);
        });

        builder.Entity<CartItem>(entity =>
        {
            entity.Property(item => item.UnitPrice).HasPrecision(18, 2);
            entity.HasIndex(item => new { item.CartId, item.ProductId }).IsUnique();
            entity.HasOne(item => item.Product)
                .WithMany()
                .HasForeignKey(item => item.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Order>(entity =>
        {
            entity.Property(order => order.Total).HasPrecision(18, 2);
            entity.Property(order => order.Name).HasMaxLength(160).IsRequired();
            entity.Property(order => order.CardOwnerName).HasMaxLength(160).IsRequired();
            entity.Property(order => order.ExpirationDate).HasMaxLength(16).IsRequired();
            entity.Property(order => order.Address).HasMaxLength(250).IsRequired();
            entity.Property(order => order.Neighborhood).HasMaxLength(120).IsRequired();
            entity.Property(order => order.Cep).HasMaxLength(16).IsRequired();
            entity.Property(order => order.City).HasMaxLength(120).IsRequired();
            entity.Property(order => order.Cpf).HasMaxLength(20).IsRequired();
            entity.Property(order => order.Complement).HasMaxLength(250);
            entity.HasOne(order => order.State)
                .WithMany()
                .HasForeignKey(order => order.StateId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<OrderItem>(entity =>
        {
            entity.Property(item => item.UnitPrice).HasPrecision(18, 2);
            entity.HasOne(item => item.Product)
                .WithMany()
                .HasForeignKey(item => item.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<CarouselImage>(entity =>
        {
            entity.Property(image => image.FileName).HasMaxLength(260).IsRequired();
        });

        builder.Entity<ProductImportJob>(entity =>
        {
            entity.Property(job => job.OriginalFileName).HasMaxLength(260).IsRequired();
            entity.Property(job => job.StoredFileName).HasMaxLength(120).IsRequired();
            entity.Property(job => job.StoredFilePath).HasMaxLength(500).IsRequired();
            entity.Property(job => job.ContentType).HasMaxLength(120).IsRequired();
            entity.Property(job => job.ImportedByName).HasMaxLength(180).IsRequired();
            entity.Property(job => job.SummaryMessage).HasMaxLength(2000);
            entity.Property(job => job.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.HasIndex(job => job.Status);
            entity.HasIndex(job => job.CreatedAt);
            entity.HasMany(job => job.Items)
                .WithOne(item => item.Job)
                .HasForeignKey(item => item.JobId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ProductImportJobItem>(entity =>
        {
            entity.Property(item => item.Sku).HasMaxLength(80);
            entity.Property(item => item.Title).HasMaxLength(250);
            entity.Property(item => item.ErrorMessage).HasMaxLength(4000);
            entity.Property(item => item.DownloadedImages).HasMaxLength(4000);
            entity.Property(item => item.ImportedAttributes).HasMaxLength(4000);
            entity.Property(item => item.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.HasIndex(item => new { item.JobId, item.RowNumber });
            entity.HasIndex(item => item.Sku);
            entity.HasIndex(item => item.Status);
        });
    }
}
