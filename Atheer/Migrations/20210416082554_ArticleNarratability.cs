using Microsoft.EntityFrameworkCore.Migrations;

namespace Atheer.Migrations
{
    public partial class ArticleNarratability : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Narratable",
                table: "Article",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NarrationMp3Url",
                table: "Article",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Narratable",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "NarrationMp3Url",
                table: "Article");
        }
    }
}
