using DeliveryPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryPlatform.Data.Configurations
{
    public class AddressConfiguration
        : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> entity)
        {
            entity.ToTable("Address");

            entity.HasKey(a => a.Id);

            entity.Property(a => a.Label)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.Latitude)
                .HasPrecision(9, 5);

            entity.Property(a => a.Longitude)
                .HasPrecision(9, 5);

            entity.HasOne(a => a.Customer)
                .WithMany()
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}