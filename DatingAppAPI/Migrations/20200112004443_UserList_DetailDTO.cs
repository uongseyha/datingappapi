using Microsoft.EntityFrameworkCore.Migrations;

namespace DatingAppAPI.Migrations
{
    public partial class UserList_DetailDTO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KnowAs",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "KnownAs",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KnownAs",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "KnowAs",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
