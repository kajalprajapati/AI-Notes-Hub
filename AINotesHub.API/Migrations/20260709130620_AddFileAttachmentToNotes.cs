using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AINotesHub.API.Migrations
{
    /// <inheritdoc />
    public partial class AddFileAttachmentToNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachmentFileName",
                table: "Notes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentPath",
                table: "Notes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentFileName",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "AttachmentPath",
                table: "Notes");
        }
    }
}
