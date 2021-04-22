using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Atheer.Migrations
{
    public partial class ToTimestampInsteadOfText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "User",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoggedInAt",
                table: "User",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Tag",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "Tag",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ArticleSeries",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Article",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Article",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.Sql(
                @$"
                UPDATE ""Article"" SET ""CreatedAt"" = CAST (to_timestamp(""CreationDate"", '(YYYY-MM-DD)T(HH24:MI:ss)') AS timestamp);
                UPDATE ""Article"" SET ""UpdatedAt"" = CAST (to_timestamp(""LastUpdatedDate"", '(YYYY-MM-DD)T(HH24:MI:ss)') AS timestamp);
                UPDATE ""ArticleSeries"" SET ""CreatedAt"" = CAST (to_timestamp(""DateCreated"", '(YYYY-MM-DD)T(HH24:MI:ss)') AS timestamp);
                UPDATE ""Tag"" SET ""CreatedAt"" = CAST (to_timestamp(""DateCreated"", '(YYYY-MM-DD)T(HH24:MI:ss)') AS timestamp);
                UPDATE ""Tag"" SET ""LastUpdatedAt"" = CAST (to_timestamp(""DateLastAddedTo"", '(YYYY-MM-DD)T(HH24:MI:ss)') AS timestamp);
                UPDATE ""User"" SET ""CreatedAt"" = CAST (to_timestamp(""DateCreated"", '(YYYY-MM-DD)T(HH24:MI:ss)') AS timestamp);
                UPDATE ""User"" SET ""LastLoggedInAt"" = CAST (to_timestamp(""DateLastLoggedIn"", '(YYYY-MM-DD)T(HH24:MI:ss)') AS timestamp);
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LastLoggedInAt",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ArticleSeries");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Article");
        }
    }
}
