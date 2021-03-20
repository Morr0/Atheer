using Microsoft.EntityFrameworkCore.Migrations;

namespace Atheer.Migrations
{
    public partial class OAuthUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OAuthProvider",
                table: "User",
                type: "varchar(16)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OAuthUser",
                table: "User",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OAuthProvider",
                table: "User");

            migrationBuilder.DropColumn(
                name: "OAuthUser",
                table: "User");
        }
    }
}
