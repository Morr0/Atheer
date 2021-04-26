using Microsoft.EntityFrameworkCore.Migrations;

namespace Atheer.Migrations
{
    public partial class SmallChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Article_Scheduled",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "Scheduled",
                table: "Article");

            migrationBuilder.AddColumn<bool>(
                name: "ForceFullyUnlisted",
                table: "Article",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForceFullyUnlisted",
                table: "Article");

            migrationBuilder.AddColumn<bool>(
                name: "Scheduled",
                table: "Article",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Article_Scheduled",
                table: "Article",
                column: "Scheduled",
                filter: "\"Scheduled\" IS TRUE");
        }
    }
}
