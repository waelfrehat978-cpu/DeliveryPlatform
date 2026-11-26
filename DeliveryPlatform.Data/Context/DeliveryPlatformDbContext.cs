using DeliveryPlatform.Data.Auditing;
using DeliveryPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
namespace DeliveryPlatform.Data.Context;

public class DeliveryPlatformDbContext : DbContext
{
    public DeliveryPlatformDbContext(
        DbContextOptions<DeliveryPlatformDbContext> options)
        : base(options)
    {
    }
    public DbSet<ProductAuditTrail> ProductAuditTrails { get; set; }

    public DbSet<OrderAuditTrail> OrderAuditTrails { get; set; }

    public DbSet<CustomerAuditTrail> CustomerAuditTrails { get; set; }

    public DbSet<StoreAuditTrail> StoreAuditTrails { get; set; }
    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Address> Addresses => Set<Address>();

    public DbSet<Store> Stores => Set<Store>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<ProductOptionGroup> ProductOptionGroups
        => Set<ProductOptionGroup>();

    public DbSet<ProductOption> ProductOptions
        => Set<ProductOption>();

    public DbSet<Cart> Carts => Set<Cart>();

    public DbSet<CartLine> CartLines => Set<CartLine>();

    public DbSet<CartLineOption> CartLineOptions
        => Set<CartLineOption>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    public DbSet<OrderLineOption> OrderLineOptions
        => Set<OrderLineOption>();

    public DbSet<OrderStatusHistory> OrderStatusHistories
        => Set<OrderStatusHistory>();
    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasSequence<long>("OrderCodeSequence")
        .StartsAt(1)
        .IncrementsBy(1)
        .HasMax(99_999_999);

        // Apply all entity configurations in this assembly.
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(DeliveryPlatformDbContext).Assembly);

        // Apply the global soft-delete filter to BaseEntity entities.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(DeliveryPlatformDbContext)
                    .GetMethod(
                        nameof(SetSoftDeleteFilter),
                        BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(null, new object[] { modelBuilder });
            }
        }
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);

            entity.Property(rt => rt.TokenHash)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasIndex(rt => rt.TokenHash)
                .IsUnique();

            entity.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(rt => rt.ReplacedByToken)
                .WithMany()
                .HasForeignKey(rt => rt.ReplacedByTokenId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void SetSoftDeleteFilter<TEntity>(
        ModelBuilder modelBuilder)
        where TEntity : BaseEntity
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(entity => !entity.IsDeleted);
    }
}
