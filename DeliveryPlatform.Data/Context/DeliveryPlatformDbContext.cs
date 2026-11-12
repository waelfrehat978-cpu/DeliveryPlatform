using System.Reflection;
using DeliveryPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;
namespace DeliveryPlatform.Data.Context;

public class DeliveryPlatformDbContext : DbContext
{
    public DeliveryPlatformDbContext(
        DbContextOptions<DeliveryPlatformDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Address> Addresses => Set<Address>();

    public DbSet<Store> Stores => Set<Store>();

    public DbSet<Category> Category => Set<Category>();

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

    public DbSet<OrderStatusHistory> OrderStatusHistory
        => Set<OrderStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
    }

    private static void SetSoftDeleteFilter<TEntity>(
        ModelBuilder modelBuilder)
        where TEntity : BaseEntity
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(entity => !entity.IsDeleted);
    }
}