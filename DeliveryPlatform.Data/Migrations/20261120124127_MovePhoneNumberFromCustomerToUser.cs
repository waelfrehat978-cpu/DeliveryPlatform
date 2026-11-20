using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryPlatform.Data.Migrations
{
    /// <inheritdoc />
    public partial class MovePhoneNumberFromCustomerToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
        UPDATE [User]
        SET [User].[PhoneNumber] = [Customer].[PhoneNumber]
        FROM [User]
        INNER JOIN [Customer]
            ON [User].[CustomerId] = [Customer].[Id]
        WHERE [User].[Role] = 0;
        """);

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Customer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Customer",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
        UPDATE [Customer]
        SET [Customer].[PhoneNumber] = [User].[PhoneNumber]
        FROM [Customer]
        INNER JOIN [User]
            ON [Customer].[Id] = [User].[CustomerId]
        WHERE [User].[Role] = 0;
        """);
        }
    }
}
