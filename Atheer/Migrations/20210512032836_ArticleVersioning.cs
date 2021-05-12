using Atheer.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace Atheer.Migrations
{
    public partial class ArticleVersioning : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "Article",
                type: "tsvector",
                nullable: true,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector",
                oldNullable: true)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Title", "Description" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "Title", "Description", "Content" });

            migrationBuilder.AddColumn<bool>(
                name: "EverPublished",
                table: "Article",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Article",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            
            migrationBuilder.Sql(@$"UPDATE ""Article"" SET ""EverPublished"" = true;");
            
            migrationBuilder.Sql($@"
            CREATE OR REPLACE VIEW popular_tags_view AS
            SELECT T.""Id"", T.""Title"", count(""TagId"") AS ""Count""
            FROM ""TagArticle"" TA
            INNER JOIN ""Tag"" T on TA.""TagId"" = T.""Id""
            INNER JOIN ""Article"" A on TA.""ArticleCreatedYear"" = A.""CreatedYear"" AND TA.""ArticleTitleShrinked"" = A.""TitleShrinked""
            WHERE A.""Unlisted"" IS false AND A.""Draft"" IS false AND ""EverPublished"" IS true AND ""ForceFullyUnlisted"" IS false
            GROUP BY T.""Id""
            ORDER BY count(""TagId"") DESC
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
            
            migrationBuilder.DropColumn(
                name: "EverPublished",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Article");

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "Article",
                type: "tsvector",
                nullable: true,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector",
                oldNullable: true)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Title", "Description", "Content" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "Title", "Description" });
        }
    }
}