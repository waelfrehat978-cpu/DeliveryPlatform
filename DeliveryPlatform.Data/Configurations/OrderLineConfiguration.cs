using DeliveryPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryPlatform.Data.Configurations
{
    public class OrderLineConfiguration
        : IEntityTypeConfiguration<OrderLine>
    {
        public void Configure(EntityTypeBuilder<OrderLine> entity)
        {
            entity.ToTable("OrderLine");

            entity.HasKey(l => l.Id);

            entity.Property(l => l.ProductName)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(l => l.Quantity)
                .IsRequired();

            entity.Property(l => l.UnitPrice)
                .HasPrecision(10, 2);

            entity.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_OrderLines_Quantity",
                    "[Quantity] > 0");

                t.HasCheckConstraint(
                    "CK_OrderLines_UnitPrice",
                    "[UnitPrice] >= 0");
            });

            entity.HasOne(l => l.Order)
                .WithMany(o =>o.OrderLines)
                .HasForeignKey(l => l.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(l => l.Product)
                .WithMany()
                .HasForeignKey(l => l.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}