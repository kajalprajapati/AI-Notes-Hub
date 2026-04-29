using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AINotesHub.API.Migrations
{
    /// <inheritdoc />
    public partial class FixStaticUserSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$7gWpzjzzpK8hM3rK9z05ueLwUjxl8eM8zFqUO5Saxn0jvJCKyMZte");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "PasswordHash",
                value: "$2a$11$yphGvDlHqT7e7dgrJgrXhe3yE3KMuapwF8OZJUn1n7b1ZXnpXUGTi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "$2a$11$pgqf.H1ztbqLhFlxQ2UfA.h9Y5erDW3rUguX7LLD1sB5dtZGoFP8C");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$T2qVWBuCThI3HXLGLHEdaOQojCDioYhICbr7cSSj4b0PJj31r.PHC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "PasswordHash",
                value: "$2a$11$2XCXMQ0W/FARcJLplTcDWO6qdPYl.GxMsDw7U1ioLTglQhYYPYb46");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "$2a$11$IVVKI8kLFJeI2Vq9sG5lguT2vAMrj043E54wd4oAIMSLkevypgr4y");
        }
    }
}
