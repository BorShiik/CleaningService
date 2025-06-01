using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanDeal.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusAndCleaner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CleanerId",
                table: "CleaningOrders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "CleaningOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_CleaningOrders_CleanerId",
                table: "CleaningOrders",
                column: "CleanerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CleaningOrders_AspNetUsers_CleanerId",
                table: "CleaningOrders",
                column: "CleanerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CleaningOrders_AspNetUsers_CleanerId",
                table: "CleaningOrders");

            migrationBuilder.DropIndex(
                name: "IX_CleaningOrders_CleanerId",
                table: "CleaningOrders");

            migrationBuilder.DropColumn(
                name: "CleanerId",
                table: "CleaningOrders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CleaningOrders");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);
        }
    }
}
