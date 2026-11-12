using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryPlatform.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReviewOneSchemaFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderStatusHistorie_Order_OrderId",
                table: "OrderStatusHistorie");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Categorie_CategoryId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Customer_Email",
                table: "Customer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderStatusHistorie",
                table: "OrderStatusHistorie");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OrderStatusHistories_Status",
                table: "OrderStatusHistorie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categorie",
                table: "Categorie");

            migrationBuilder.DropColumn(
                name: "ChangedAtUtc",
                table: "OrderStatusHistorie");

            migrationBuilder.RenameTable(
                name: "OrderStatusHistorie",
                newName: "OrderStatusHistory");

            migrationBuilder.RenameTable(
                name: "Categorie",
                newName: "Category");

            migrationBuilder.RenameIndex(
                name: "IX_OrderStatusHistorie_OrderId",
                table: "OrderStatusHistory",
                newName: "IX_OrderStatusHistory_OrderId");

            migrationBuilder.AlterColumn<int>(
                name: "AddressId",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Customer",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Customer",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Customer",
                type: "nvarchar(254)",
                maxLength: 254,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(254)",
                oldMaxLength: 254,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldStatus",
                table: "OrderStatusHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderStatusHistory",
                table: "OrderStatusHistory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Email",
                table: "Customer",
                column: "Email",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrderStatusHistory_OldStatus",
                table: "OrderStatusHistory",
                sql: "[OldStatus] IS NULL OR [OldStatus] IN (0, 1, 2, 3, 4, 5, 6)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrderStatusHistory_Status",
                table: "OrderStatusHistory",
                sql: "[Status] IN (0, 1, 2, 3, 4, 5, 6)");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderStatusHistory_Order_OrderId",
                table: "OrderStatusHistory",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Category_CategoryId",
                table: "Product",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderStatusHistory_Order_OrderId",
                table: "OrderStatusHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Category_CategoryId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Customer_Email",
                table: "Customer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderStatusHistory",
                table: "OrderStatusHistory");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OrderStatusHistory_OldStatus",
                table: "OrderStatusHistory");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OrderStatusHistory_Status",
                table: "OrderStatusHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "OldStatus",
                table: "OrderStatusHistory");

            migrationBuilder.RenameTable(
                name: "OrderStatusHistory",
                newName: "OrderStatusHistorie");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "Categorie");

            migrationBuilder.RenameIndex(
                name: "IX_OrderStatusHistory_OrderId",
                table: "OrderStatusHistorie",
                newName: "IX_OrderStatusHistorie_OrderId");

            migrationBuilder.AlterColumn<int>(
                name: "AddressId",
                table: "Order",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Customer",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Customer",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Customer",
                type: "nvarchar(254)",
                maxLength: 254,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(254)",
                oldMaxLength: 254);

            migrationBuilder.AddColumn<DateTime>(
                name: "ChangedAtUtc",
                table: "OrderStatusHistorie",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderStatusHistorie",
                table: "OrderStatusHistorie",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categorie",
                table: "Categorie",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Email",
                table: "Customer",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrderStatusHistories_Status",
                table: "OrderStatusHistorie",
                sql: "[Status] IN (0, 1, 2, 3, 4, 5, 6)");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderStatusHistorie_Order_OrderId",
                table: "OrderStatusHistorie",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Categorie_CategoryId",
                table: "Product",
                column: "CategoryId",
                principalTable: "Categorie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
