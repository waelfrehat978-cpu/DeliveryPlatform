using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryPlatform.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSignupTokenHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SignupTokenHash",
                table: "OtpCodes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignupTokenHash",
                table: "OtpCodes");
        }
    }
}
