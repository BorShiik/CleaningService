using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanDeal.Migrations
{
    /// <inheritdoc />
    public partial class AddMultipleServiceTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CleaningOrders_ServiceTypes_ServiceTypeId",
                table: "CleaningOrders");

            migrationBuilder.CreateTable(
                name: "CleaningOrderServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CleaningOrderId = table.Column<int>(type: "int", nullable: false),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CleaningOrderServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CleaningOrderServices_CleaningOrders_CleaningOrderId",
                        column: x => x.CleaningOrderId,
                        principalTable: "CleaningOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CleaningOrderServices_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CleaningOrderServices_CleaningOrderId",
                table: "CleaningOrderServices",
                column: "CleaningOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CleaningOrderServices_ServiceTypeId",
                table: "CleaningOrderServices",
                column: "ServiceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CleaningOrders_ServiceTypes_ServiceTypeId",
                table: "CleaningOrders",
                column: "ServiceTypeId",
                principalTable: "ServiceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CleaningOrders_ServiceTypes_ServiceTypeId",
                table: "CleaningOrders");

            migrationBuilder.DropTable(
                name: "CleaningOrderServices");

            migrationBuilder.AddForeignKey(
                name: "FK_CleaningOrders_ServiceTypes_ServiceTypeId",
                table: "CleaningOrders",
                column: "ServiceTypeId",
                principalTable: "ServiceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
