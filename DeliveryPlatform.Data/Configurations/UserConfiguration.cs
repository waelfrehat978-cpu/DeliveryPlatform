using DeliveryPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryPlatform.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User", table =>
        {
            table.HasCheckConstraint(
                "CK_User_Role_Valid",
                "[Role] IN (0, 1, 2)");

            table.HasCheckConstraint(
                "CK_User_Customer_Fields",
                """
        ([Role] = 0
            AND [PhoneNumber] IS NOT NULL
            AND [Email] IS NULL
            AND [PasswordHash] IS NULL
            AND [CustomerId] IS NOT NULL
            AND [StoreId] IS NULL)
        OR
        ([Role] = 1
            AND [PhoneNumber] IS NULL
            AND [Email] IS NOT NULL
            AND [PasswordHash] IS NOT NULL
            AND [CustomerId] IS NULL
            AND [StoreId] IS NOT NULL)
        OR
        ([Role] = 2
            AND [PhoneNumber] IS NULL
            AND [Email] IS NOT NULL
            AND [PasswordHash] IS NOT NULL
            AND [CustomerId] IS NULL
            AND [StoreId] IS NULL)
        """);
        });

        builder.HasKey(u => u.Id);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(u => u.Email)
            .HasMaxLength(254);

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(500);

        builder.Property(u => u.Role)
            .IsRequired();


        // User - Customer (One-to-One)
        builder.HasOne(u => u.Customer)
            .WithOne()
            .HasForeignKey<User>(u => u.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // User - Store (One-to-One)
        builder.HasOne(u => u.Store)
            .WithOne()
            .HasForeignKey<User>(u => u.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(u => u.PhoneNumber)
    .IsUnique()
    .HasFilter("[PhoneNumber] IS NOT NULL");

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter("[Email] IS NOT NULL");

        builder.HasIndex(u => u.CustomerId)
            .IsUnique()
            .HasFilter("[CustomerId] IS NOT NULL");

        builder.HasIndex(u => u.StoreId)
            .IsUnique()
            .HasFilter("[StoreId] IS NOT NULL");


    }
}