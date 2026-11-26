using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryPlatform.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAuditStamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Store",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "Store",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "Store",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "RefreshTokens",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "RefreshTokens",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "RefreshTokens",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "ProductOptionGroup",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "ProductOptionGroup",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "ProductOptionGroup",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "ProductOption",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "ProductOption",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "ProductOption",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "OtpCodes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "OtpCodes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "OtpCodes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "OrderStatusHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "OrderStatusHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "OrderStatusHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "OrderLineOption",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "OrderLineOption",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "OrderLineOption",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "OrderLine",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "OrderLine",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "OrderLine",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Customer",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "Customer",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "Customer",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Category",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "Category",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "Category",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "CartLineOption",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "CartLineOption",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "CartLineOption",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "CartLine",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "CartLine",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "CartLine",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Cart",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "Cart",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "Cart",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Address",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "Address",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "Address",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ProductOptionGroup");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "ProductOptionGroup");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ProductOptionGroup");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ProductOption");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "ProductOption");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ProductOption");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "OtpCodes");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "OtpCodes");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "OtpCodes");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "OrderStatusHistory");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "OrderStatusHistory");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "OrderStatusHistory");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "OrderLineOption");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "OrderLineOption");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "OrderLineOption");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "OrderLine");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "OrderLine");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "OrderLine");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "CartLineOption");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "CartLineOption");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "CartLineOption");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "CartLine");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "CartLine");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "CartLine");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Address");
        }
    }
}
