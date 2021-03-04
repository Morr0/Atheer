using Microsoft.EntityFrameworkCore.Migrations;

namespace Atheer.Migrations
{
    public partial class RemovedUnnecessaryDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_Verified",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VerificationDate",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Verified",
                table: "User");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VerificationDate",
                table: "User",
                type: "varchar(20)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Verified",
                table: "User",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_User_Verified",
                table: "User",
                column: "Verified",
                filter: "\"Verified\" IS FALSE");
        }
    }
}
