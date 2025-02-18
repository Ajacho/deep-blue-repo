using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uxcheckmate_Main.Migrations
{
    /// <inheritdoc />
    public partial class MakeUserIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_UserID",
                table: "Reports");

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "Reports",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_UserID",
                table: "Reports",
                column: "UserID",
                principalTable: "UserAccounts",
                principalColumn: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_UserID",
                table: "Reports");

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "Reports",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_UserID",
                table: "Reports",
                column: "UserID",
                principalTable: "UserAccounts",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
