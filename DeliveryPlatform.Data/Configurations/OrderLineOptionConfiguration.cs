using DeliveryPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryPlatform.Data.Configurations
{
    public class OrderLineOptionConfiguration
        : IEntityTypeConfiguration<OrderLineOption>
    {
        public void Configure(EntityTypeBuilder<OrderLineOption> entity)
        {
            entity.ToTable("OrderLineOption");

            entity.HasKey(o => o.Id);

            entity.HasOne(o => o.OrderLine)
                .WithMany()
                .HasForeignKey(o => o.OrderLineId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.ProductOption)
                .WithMany()
                .HasForeignKey(o => o.ProductOptionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}