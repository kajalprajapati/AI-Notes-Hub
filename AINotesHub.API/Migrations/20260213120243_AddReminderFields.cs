using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AINotesHub.API.Migrations
{
    /// <inheritdoc />
    public partial class AddReminderFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReminderTime",
                table: "Notes",
                newName: "ReminderDateTime");

            migrationBuilder.AddColumn<bool>(
                name: "IsReminderOn",
                table: "Notes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReminderOn",
                table: "Notes");

            migrationBuilder.RenameColumn(
                name: "ReminderDateTime",
                table: "Notes",
                newName: "ReminderTime");
        }
    }
}
