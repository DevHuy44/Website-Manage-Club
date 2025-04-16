using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BussinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNotificationMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
              name: "message",
              table: "Notifications",
              type: "nvarchar(500)",
              nullable: false,
              oldClrType: typeof(string),
              oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
              name: "message",
              table: "Notifications",
              type: "text",
              nullable: false,
              oldClrType: typeof(string),
              oldType: "nvarchar(500)");
        }
    }
}
