using Microsoft.EntityFrameworkCore.Migrations;

namespace Team_EasyEntry.Data.Migrations
{
    public partial class initialsetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    FirstShotDate = table.Column<string>(nullable: true),
                    FirstShotName = table.Column<string>(nullable: true),
                    SecondShotDate = table.Column<string>(nullable: true),
                    SecondShotName = table.Column<string>(nullable: true),
                    ThirdShotDate = table.Column<string>(nullable: true),
                    ThirdShotName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customer");
        }
    }
}
