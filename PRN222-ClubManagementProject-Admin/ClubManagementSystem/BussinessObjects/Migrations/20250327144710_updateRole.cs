using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BussinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class updateRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClubId",
                table: "Roles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_ClubId",
                table: "Roles",
                column: "ClubId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Clubs_ClubId",
                table: "Roles",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "club_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Clubs_ClubId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_ClubId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "ClubId",
                table: "Roles");
        }
    }
}
