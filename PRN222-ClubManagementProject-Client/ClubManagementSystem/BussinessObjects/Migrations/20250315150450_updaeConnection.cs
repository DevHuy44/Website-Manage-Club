using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BussinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class updaeConnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connection_Users_UserId1",
                table: "Connection");

            migrationBuilder.DropIndex(
                name: "IX_Connection_UserId1",
                table: "Connection");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Connection");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Connection",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Connection_UserId",
                table: "Connection",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Connection_Users_UserId",
                table: "Connection",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connection_Users_UserId",
                table: "Connection");

            migrationBuilder.DropIndex(
                name: "IX_Connection_UserId",
                table: "Connection");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Connection",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Connection",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Connection_UserId1",
                table: "Connection",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Connection_Users_UserId1",
                table: "Connection",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "user_id");
        }
    }
}
