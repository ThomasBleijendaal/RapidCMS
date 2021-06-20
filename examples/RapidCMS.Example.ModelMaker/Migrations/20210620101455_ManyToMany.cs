using Microsoft.EntityFrameworkCore.Migrations;

namespace RapidCMS.Example.ModelMaker.Migrations
{
    public partial class ManyToMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManytoManyManyAs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManytoManyManyAs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ManytoManyManyBs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManytoManyManyBs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ManytoManyManyAManytoManyManyB",
                columns: table => new
                {
                    AsId = table.Column<int>(type: "int", nullable: false),
                    BsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManytoManyManyAManytoManyManyB", x => new { x.AsId, x.BsId });
                    table.ForeignKey(
                        name: "FK_ManytoManyManyAManytoManyManyB_ManytoManyManyAs_AsId",
                        column: x => x.AsId,
                        principalTable: "ManytoManyManyAs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManytoManyManyAManytoManyManyB_ManytoManyManyBs_BsId",
                        column: x => x.BsId,
                        principalTable: "ManytoManyManyBs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ManytoManyManyAManytoManyManyB_BsId",
                table: "ManytoManyManyAManytoManyManyB",
                column: "BsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManytoManyManyAManytoManyManyB");

            migrationBuilder.DropTable(
                name: "ManytoManyManyAs");

            migrationBuilder.DropTable(
                name: "ManytoManyManyBs");
        }
    }
}
