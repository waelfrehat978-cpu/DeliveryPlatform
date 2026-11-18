
using DeliveryPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryPlatform.Data.Configurations;

public class OtpCodeConfiguration : IEntityTypeConfiguration<OtpCode>
{
    public void Configure(EntityTypeBuilder<OtpCode> builder)
    {
        builder.ToTable("OtpCodes");

        builder.Property(o => o.PhoneNumber)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(o => o.Code)
            .IsRequired()
            .HasMaxLength(6);

        builder.Property(o => o.ExpiresAtUtc)
            .IsRequired();

        builder.Property(o => o.WrongAttempts)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(o => o.IsUsed)
            .IsRequired()
            .HasDefaultValue(false);
        builder.Property(o => o.VerifiedAtUtc)
    .IsRequired(false);

        builder.Property(o => o.IsSignupCompleted)
            .IsRequired()
            .HasDefaultValue(false);
        builder.HasIndex(o => new { o.PhoneNumber, o.ExpiresAtUtc });
    }
}
