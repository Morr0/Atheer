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
                    AuthorId = table.Column<string>(type: "varchar(64)", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "varchar(512)", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    CreationDate = table.Column<string>(type: "varchar(20)", nullable: true),
                    LastUpdatedDate = table.Column<string>(type: "varchar(20)", nullable: true),
                    Likeable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Likes = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Shareable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Shares = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Draft = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Unlisted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Scheduled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ScheduledSinceDate = table.Column<string>(type: "varchar(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => new { x.CreatedYear, x.TitleShrinked });
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "varchar(64)", nullable: true),
                    DateCreated = table.Column<string>(type: "varchar(20)", nullable: true),
                    DateLastAddedTo = table.Column<string>(type: "varchar(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(32)", nullable: true),
                    LastName = table.Column<string>(type: "varchar(32)", nullable: true),
                    Bio = table.Column<string>(type: "varchar(512)", nullable: true),
                    Email = table.Column<string>(type: "varchar(192)", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<string>(type: "varchar(20)", nullable: true),
                    DateLastLoggedIn = table.Column<string>(type: "varchar(20)", nullable: true),
                    Roles = table.Column<string>(type: "text", nullable: true),
                    Verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    VerificationDate = table.Column<string>(type: "varchar(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
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
                name: "IX_Article_Scheduled",
                table: "Article",
                column: "Scheduled",
                filter: "\"Scheduled\" IS TRUE");

            migrationBuilder.CreateIndex(
                name: "IX_TagArticle_ArticleCreatedYear_ArticleTitleShrinked",
                table: "TagArticle",
                columns: new[] { "ArticleCreatedYear", "ArticleTitleShrinked" });

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
                name: "TagArticle");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Article");

            migrationBuilder.DropTable(
                name: "Tag");
        }
    }
}
