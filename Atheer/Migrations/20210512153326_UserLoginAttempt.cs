using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Atheer.Migrations
{
    public partial class UserLoginAttempt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserLoginAttempt",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    AttemptAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ReferenceId = table.Column<string>(type: "text", nullable: true),
                    SuccessfulLogin = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoginAttempt", x => new { x.UserId, x.AttemptAt });
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginAttempt_UserId",
                table: "UserLoginAttempt",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLoginAttempt");
        }
    }
}
