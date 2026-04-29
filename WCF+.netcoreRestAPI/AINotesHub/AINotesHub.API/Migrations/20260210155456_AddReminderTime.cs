using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AINotesHub.API.Migrations
{
    /// <inheritdoc />
    public partial class AddReminderTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReminderTime",
                table: "Notes",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReminderTime",
                table: "Notes");
        }
    }
}
