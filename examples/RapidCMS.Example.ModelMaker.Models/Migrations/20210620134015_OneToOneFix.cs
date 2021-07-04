using Microsoft.EntityFrameworkCore.Migrations;

namespace RapidCMS.Example.ModelMaker.Migrations
{
    public partial class OneToOneFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AId",
                table: "OnetoOneOneBs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AId",
                table: "OnetoOneOneBs",
                type: "int",
                nullable: true);
        }
    }
}
