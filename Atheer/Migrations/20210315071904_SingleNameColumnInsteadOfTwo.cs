using Microsoft.EntityFrameworkCore.Migrations;

namespace Atheer.Migrations
{
    public partial class SingleNameColumnInsteadOfTwo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "User",
                type: "varchar(64)",
                nullable: true,
                computedColumnSql: "\"FirstName\" || ' ' || \"LastName\"",
                stored: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "User");
        }
    }
}
