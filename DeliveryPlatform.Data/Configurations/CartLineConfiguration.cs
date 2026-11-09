using DeliveryPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryPlatform.Data.Configurations
{
    public class CartLineConfiguration
        : IEntityTypeConfiguration<CartLine>
    {
        public void Configure(EntityTypeBuilder<CartLine> entity)
        {
            entity.ToTable("CartLine");

            entity.HasKey(l => l.Id);

            entity.Property(l => l.Quantity)
                .IsRequired();

            entity.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_CartLines_Quantity",
                    "[Quantity] > 0");
            });

            entity.HasOne(l => l.Cart)
                .WithMany()
                .HasForeignKey(l => l.CartId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(l => l.Product)
                .WithMany()
                .HasForeignKey(l => l.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}