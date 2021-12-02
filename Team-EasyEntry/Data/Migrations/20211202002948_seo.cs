using Microsoft.EntityFrameworkCore.Migrations;

namespace Team_EasyEntry.Data.Migrations
{
    public partial class seo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Vaccinated",
                table: "Customer",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Vaccinated",
                table: "Customer");
        }
    }
}
