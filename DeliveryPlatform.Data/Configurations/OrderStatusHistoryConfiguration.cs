using DeliveryPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryPlatform.Data.Configurations
{
    public class OrderStatusHistoryConfiguration
        : IEntityTypeConfiguration<OrderStatusHistory>
    {
        public void Configure(
            EntityTypeBuilder<OrderStatusHistory> entity)
        {
            entity.ToTable("OrderStatusHistorie");

            entity.HasKey(h => h.Id);

            entity.Property(h => h.Note)
                .HasMaxLength(500);

            entity.Property(h => h.ChangedAtUtc)
                .IsRequired();

            entity.Property(h => h.Status)
                .IsRequired();

            entity.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_OrderStatusHistories_Status",
                    "[Status] IN (0, 1, 2, 3, 4, 5, 6)");
            });

            entity.HasOne(h => h.Order)
                .WithMany()
                .HasForeignKey(h => h.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}