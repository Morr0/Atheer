using Atheer.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Atheer.Migrations
{
    public partial class NameInsteadOfFirstLastName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "User",
                type: "varchar(64)",
                nullable: true);

            migrationBuilder.Sql(
                $"UPDATE \"User\" SET \"{nameof(User.Name)}\" = (\"{nameof(User.FirstName)}\" || ' ' || \"{nameof(User.LastName)}\");");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "User");
        }
    }
}
