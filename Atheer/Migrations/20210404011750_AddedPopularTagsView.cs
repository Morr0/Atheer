using Microsoft.EntityFrameworkCore.Migrations;

namespace Atheer.Migrations
{
    public partial class AddedPopularTagsView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE OR REPLACE VIEW popular_tags_view AS
            SELECT T.""Id"", T.""Title"", count(""TagId"") AS ""Count""
            FROM ""TagArticle"" TA
            INNER JOIN ""Tag"" T on TA.""TagId"" = T.""Id""
            INNER JOIN ""Article"" A on TA.""ArticleCreatedYear"" = A.""CreatedYear"" AND TA.""ArticleTitleShrinked"" = A.""TitleShrinked""
            WHERE A.""Unlisted"" IS false AND A.""Draft"" IS false
            GROUP BY T.""Id""
            ORDER BY count(""TagId"") DESC
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW popular_tags_view");
        }
    }
}
