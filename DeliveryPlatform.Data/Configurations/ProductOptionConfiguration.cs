using DeliveryPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryPlatform.Data.Configurations
{
    public class ProductOptionConfiguration
        : IEntityTypeConfiguration<ProductOption>
    {
        public void Configure(
            EntityTypeBuilder<ProductOption> entity)
        {
            entity.ToTable("ProductOption");

            entity.HasKey(o => o.Id);

            entity.Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(o => o.AdditionalPrice)
                .HasPrecision(10, 2);

            entity.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_ProductOptions_AdditionalPrice",
                    "[AdditionalPrice] >= 0");
            });

            entity.HasOne(o => o.ProductOptionGroup)
                .WithMany()
                .HasForeignKey(o => o.ProductOptionGroupId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}