using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BussinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class updateTask_Event : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_EventId",
                table: "Tasks",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Events_EventId",
                table: "Tasks",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "event_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Events_EventId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_EventId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Tasks");
        }
    }
}
