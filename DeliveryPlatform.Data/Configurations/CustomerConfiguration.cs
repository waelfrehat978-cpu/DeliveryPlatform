using DeliveryPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryPlatform.Data.Configurations
{
    public class CustomerConfiguration
        : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> entity)
        {
            entity.ToTable("Customer");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(c => c.LastName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(c => c.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(c => c.Email)
                .HasMaxLength(254);

            entity.HasIndex(c => c.Email)
                .IsUnique()
                .HasFilter("[Email] IS NOT NULL");
        }
    }
}