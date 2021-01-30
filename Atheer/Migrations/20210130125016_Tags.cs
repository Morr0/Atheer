using Microsoft.EntityFrameworkCore.Migrations;

namespace Atheer.Migrations
{
    public partial class Tags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<string>(type: "text", nullable: true),
                    DateLastAddedTo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TagArticle",
                columns: table => new
                {
                    TagId = table.Column<string>(type: "text", nullable: false),
                    ArticleCreatedYear = table.Column<int>(type: "integer", nullable: false),
                    ArticleTitleShrinked = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagArticle", x => new { x.TagId, x.ArticleCreatedYear, x.ArticleTitleShrinked });
                    table.ForeignKey(
                        name: "FK_TagArticle_Article_ArticleCreatedYear_ArticleTitleShrinked",
                        columns: x => new { x.ArticleCreatedYear, x.ArticleTitleShrinked },
                        principalTable: "Article",
                        principalColumns: new[] { "CreatedYear", "TitleShrinked" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagArticle_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagArticle_ArticleCreatedYear_ArticleTitleShrinked",
                table: "TagArticle",
                columns: new[] { "ArticleCreatedYear", "ArticleTitleShrinked" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagArticle");

            migrationBuilder.DropTable(
                name: "Tag");
        }
    }
}
