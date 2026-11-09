using DeliveryPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryPlatform.Data.Configurations
{
    public class ProductOptionGroupConfiguration
        : IEntityTypeConfiguration<ProductOptionGroup>
    {
        public void Configure(
            EntityTypeBuilder<ProductOptionGroup> entity)
        {
            entity.ToTable("ProductOptionGroup");

            entity.HasKey(g => g.Id);

            entity.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(g => g.Product)
                .WithMany()
                .HasForeignKey(g => g.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}