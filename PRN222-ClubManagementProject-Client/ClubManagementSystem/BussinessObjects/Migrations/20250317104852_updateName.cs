using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BussinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class updateName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connection_Users_UserId",
                table: "Connection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Connection",
                table: "Connection");

            migrationBuilder.RenameTable(
                name: "Connection",
                newName: "Connections");

            migrationBuilder.RenameIndex(
                name: "IX_Connection_UserId",
                table: "Connections",
                newName: "IX_Connections_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Connections",
                table: "Connections",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Connections_Users_UserId",
                table: "Connections",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connections_Users_UserId",
                table: "Connections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Connections",
                table: "Connections");

            migrationBuilder.RenameTable(
                name: "Connections",
                newName: "Connection");

            migrationBuilder.RenameIndex(
                name: "IX_Connections_UserId",
                table: "Connection",
                newName: "IX_Connection_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Connection",
                table: "Connection",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Connection_Users_UserId",
                table: "Connection",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
