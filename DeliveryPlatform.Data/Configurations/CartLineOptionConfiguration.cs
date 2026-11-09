using DeliveryPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryPlatform.Data.Configurations
{
    public class CartLineOptionConfiguration
        : IEntityTypeConfiguration<CartLineOption>
    {
        public void Configure(EntityTypeBuilder<CartLineOption> entity)
        {
            entity.ToTable("CartLineOption");

            entity.HasKey(o => o.Id);

            entity.HasOne(o => o.CartLine)
                .WithMany()
                .HasForeignKey(o => o.CartLineId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.ProductOption)
                .WithMany()
                .HasForeignKey(o => o.ProductOptionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}