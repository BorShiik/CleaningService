using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanDeal.Migrations
{
    /// <inheritdoc />
    public partial class AddNewAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Availabilities_AspNetUsers_CleanerId",
                table: "Availabilities");

            migrationBuilder.DropIndex(
                name: "IX_Availabilities_CleanerId",
                table: "Availabilities");

            migrationBuilder.AlterColumn<string>(
                name: "CleanerId",
                table: "Availabilities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CleanerId",
                table: "Availabilities",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Availabilities_CleanerId",
                table: "Availabilities",
                column: "CleanerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Availabilities_AspNetUsers_CleanerId",
                table: "Availabilities",
                column: "CleanerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
