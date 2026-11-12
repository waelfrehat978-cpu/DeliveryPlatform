using DeliveryPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryPlatform.Data.Configurations
{
    public class OrderConfiguration
        : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> entity)
        {
            entity.ToTable("Order");

            entity.HasKey(o => o.Id);

            entity.Property(o => o.OrderCode)
                .IsRequired()
                .HasMaxLength(12);

            entity.HasIndex(o => o.OrderCode)
                .IsUnique();

            entity.Property(o => o.Status)
                .IsRequired();

            entity.Property(o => o.PaymentMethod)
                .IsRequired();

            entity.Property(o => o.Subtotal)
                .HasPrecision(12, 2);

            entity.Property(o => o.DeliveryFee)
                .HasPrecision(10, 2);

            entity.Property(o => o.Total)
                .HasPrecision(12, 2);

            entity.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Orders_Status",
                    "[Status] IN (0, 1, 2, 3, 4, 5, 6)");

                t.HasCheckConstraint(
                    "CK_Orders_PaymentMethod",
                    "[PaymentMethod] IN (0, 1)");

                t.HasCheckConstraint(
                    "CK_Orders_Subtotal",
                    "[Subtotal] >= 0");

                t.HasCheckConstraint(
                    "CK_Orders_DeliveryFee",
                    "[DeliveryFee] >= 0");

                t.HasCheckConstraint(
                    "CK_Orders_Total",
                    "[Total] >= 0");
            });

            entity.HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Store)
                .WithMany()
                .HasForeignKey(o => o.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Address)
                .WithMany()
                .HasForeignKey(o => o.AddressId).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}