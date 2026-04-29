using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AINotesHub.API.Migrations
{
    /// <inheritdoc />
    public partial class AddStarredAndImportant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsImportant",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsStarred",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsImportant",
                table: "Notes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsStarred",
                table: "Notes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "IsImportant", "IsStarred" },
                values: new object[] { false, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "IsImportant", "IsStarred" },
                values: new object[] { false, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "IsImportant", "IsStarred" },
                values: new object[] { false, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsImportant",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsStarred",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsImportant",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "IsStarred",
                table: "Notes");
        }
    }
}
