using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Atheer.Migrations
{
    public partial class AddedArticleSeries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SeriesId",
                table: "Article",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ArticleSeries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Finished = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<string>(type: "varchar(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleSeries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleSeries_Finished",
                table: "ArticleSeries",
                column: "Finished",
                filter: "\"Finished\" IS FALSE");

            migrationBuilder.AddForeignKey(
                name: "FK_Article_ArticleSeries_SeriesId",
                table: "Article",
                column: "SeriesId",
                principalTable: "ArticleSeries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Article_ArticleSeries_SeriesId",
                table: "Article");

            migrationBuilder.DropTable(
                name: "ArticleSeries");

            migrationBuilder.DropColumn(
                name: "SeriesId",
                table: "Article");
        }
    }
}
