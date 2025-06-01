using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanDeal.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderItemsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_CleaningOrders_CleaningOrderId",
                table: "OrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_ServiceTypes_ServiceTypeId",
                table: "OrderItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItem",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "CleaningOrders");

            migrationBuilder.RenameTable(
                name: "OrderItem",
                newName: "OrderItems");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_ServiceTypeId",
                table: "OrderItems",
                newName: "IX_OrderItems_ServiceTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_CleaningOrderId",
                table: "OrderItems",
                newName: "IX_OrderItems_CleaningOrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_CleaningOrders_CleaningOrderId",
                table: "OrderItems",
                column: "CleaningOrderId",
                principalTable: "CleaningOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ServiceTypes_ServiceTypeId",
                table: "OrderItems",
                column: "ServiceTypeId",
                principalTable: "ServiceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_CleaningOrders_CleaningOrderId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ServiceTypes_ServiceTypeId",
                table: "OrderItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems");

            migrationBuilder.RenameTable(
                name: "OrderItems",
                newName: "OrderItem");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_ServiceTypeId",
                table: "OrderItem",
                newName: "IX_OrderItem_ServiceTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_CleaningOrderId",
                table: "OrderItem",
                newName: "IX_OrderItem_CleaningOrderId");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "CleaningOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItem",
                table: "OrderItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_CleaningOrders_CleaningOrderId",
                table: "OrderItem",
                column: "CleaningOrderId",
                principalTable: "CleaningOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_ServiceTypes_ServiceTypeId",
                table: "OrderItem",
                column: "ServiceTypeId",
                principalTable: "ServiceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
