using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Atheer.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    CreatedYear = table.Column<int>(type: "integer", nullable: false),
                    TitleShrinked = table.Column<string>(type: "text", nullable: false),
                    AuthorId = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    CreationDate = table.Column<string>(type: "text", nullable: true),
                    LastUpdatedDate = table.Column<string>(type: "text", nullable: true),
                    Topics = table.Column<List<string>>(type: "text[]", nullable: true),
                    Likeable = table.Column<bool>(type: "boolean", nullable: false),
                    Likes = table.Column<int>(type: "integer", nullable: false),
                    Shareable = table.Column<bool>(type: "boolean", nullable: false),
                    Shares = table.Column<int>(type: "integer", nullable: false),
                    Draft = table.Column<bool>(type: "boolean", nullable: false),
                    Unlisted = table.Column<bool>(type: "boolean", nullable: false),
                    Scheduled = table.Column<bool>(type: "boolean", nullable: false),
                    ScheduledSinceDate = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => new { x.CreatedYear, x.TitleShrinked });
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Bio = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<string>(type: "text", nullable: true),
                    DateLastLoggedIn = table.Column<string>(type: "text", nullable: true),
                    Roles = table.Column<string>(type: "text", nullable: true),
                    Verified = table.Column<bool>(type: "boolean", nullable: false),
                    VerificationDate = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Article_Scheduled",
                table: "Article",
                column: "Scheduled",
                filter: "\"Scheduled\" IS TRUE");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Verified",
                table: "User",
                column: "Verified",
                filter: "\"Verified\" IS FALSE");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Article");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
