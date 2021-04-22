using Microsoft.EntityFrameworkCore.Migrations;

namespace Atheer.Migrations
{
    public partial class RemovedTextDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "User");

            migrationBuilder.DropColumn(
                name: "DateLastLoggedIn",
                table: "User");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "DateLastAddedTo",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "ArticleSeries");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "ScheduledSinceDate",
                table: "Article");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DateCreated",
                table: "User",
                type: "varchar(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateLastLoggedIn",
                table: "User",
                type: "varchar(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateCreated",
                table: "Tag",
                type: "varchar(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateLastAddedTo",
                table: "Tag",
                type: "varchar(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateCreated",
                table: "ArticleSeries",
                type: "varchar(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreationDate",
                table: "Article",
                type: "varchar(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedDate",
                table: "Article",
                type: "varchar(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScheduledSinceDate",
                table: "Article",
                type: "varchar(20)",
                nullable: true);
        }
    }
}
