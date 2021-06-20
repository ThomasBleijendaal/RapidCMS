using Microsoft.EntityFrameworkCore.Migrations;

namespace RapidCMS.Example.ModelMaker.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OnetoManyOnes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnetoManyOnes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OnetoManyManys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OneId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnetoManyManys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnetoManyManys_OnetoManyOnes_OneId",
                        column: x => x.OneId,
                        principalTable: "OnetoManyOnes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OnetoManyManys_OneId",
                table: "OnetoManyManys",
                column: "OneId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OnetoManyManys");

            migrationBuilder.DropTable(
                name: "OnetoManyOnes");
        }
    }
}
