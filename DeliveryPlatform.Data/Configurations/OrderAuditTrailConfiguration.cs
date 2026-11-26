using DeliveryPlatform.Data.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryPlatform.Data.Configurations;

public class OrderAuditTrailConfiguration
    : IEntityTypeConfiguration<OrderAuditTrail>
{
    public void Configure(EntityTypeBuilder<OrderAuditTrail> builder)
    {
        builder.Property(x => x.EntityName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.EntityId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ChangesJson)
            .IsRequired();

        builder.HasIndex(x => new { x.EntityName, x.EntityId });
    }
}