using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanDeal.Migrations
{
    /// <inheritdoc />
    public partial class AddProductOrdersFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_CleaningOrderId",
                table: "Payments");

            migrationBuilder.AlterColumn<int>(
                name: "CleaningOrderId",
                table: "Payments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ProductOrderId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DeliveryMethod = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductOrders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductOrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductOrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductOrderItems_ProductOrders_ProductOrderId",
                        column: x => x.ProductOrderId,
                        principalTable: "ProductOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductOrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CleaningOrderId",
                table: "Payments",
                column: "CleaningOrderId",
                unique: true,
                filter: "[CleaningOrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ProductOrderId",
                table: "Payments",
                column: "ProductOrderId",
                unique: true,
                filter: "[ProductOrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductOrderItems_ProductId",
                table: "ProductOrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductOrderItems_ProductOrderId",
                table: "ProductOrderItems",
                column: "ProductOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductOrders_UserId",
                table: "ProductOrders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_ProductOrders_ProductOrderId",
                table: "Payments",
                column: "ProductOrderId",
                principalTable: "ProductOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_ProductOrders_ProductOrderId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "ProductOrderItems");

            migrationBuilder.DropTable(
                name: "ProductOrders");

            migrationBuilder.DropIndex(
                name: "IX_Payments_CleaningOrderId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_ProductOrderId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ProductOrderId",
                table: "Payments");

            migrationBuilder.AlterColumn<int>(
                name: "CleaningOrderId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CleaningOrderId",
                table: "Payments",
                column: "CleaningOrderId",
                unique: true);
        }
    }
}
