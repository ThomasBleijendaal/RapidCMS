using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestLibrary.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonContries",
                columns: table => new
                {
                    PersonId = table.Column<int>(nullable: false),
                    CountryId = table.Column<int>(nullable: false),
                    CountryEntityId = table.Column<int>(nullable: true),
                    PersonEntityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonContries", x => new { x.CountryId, x.PersonId });
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    CountryId = table.Column<int>(nullable: true),
                    CountryPersonId = table.Column<int>(nullable: true),
                    CountryEntityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Persons_PersonContries_CountryId_CountryPersonId",
                        columns: x => new { x.CountryId, x.CountryPersonId },
                        principalTable: "PersonContries",
                        principalColumns: new[] { "CountryId", "PersonId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    PersonCountryId = table.Column<int>(nullable: true),
                    PersonId = table.Column<int>(nullable: true),
                    PersonEntityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Countries_Persons_PersonEntityId",
                        column: x => x.PersonEntityId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Countries_PersonContries_PersonCountryId_PersonId",
                        columns: x => new { x.PersonCountryId, x.PersonId },
                        principalTable: "PersonContries",
                        principalColumns: new[] { "CountryId", "PersonId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Countries_PersonEntityId",
                table: "Countries",
                column: "PersonEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_PersonCountryId_PersonId",
                table: "Countries",
                columns: new[] { "PersonCountryId", "PersonId" });

            migrationBuilder.CreateIndex(
                name: "IX_PersonContries_CountryEntityId",
                table: "PersonContries",
                column: "CountryEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonContries_PersonEntityId",
                table: "PersonContries",
                column: "PersonEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_CountryEntityId",
                table: "Persons",
                column: "CountryEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_CountryId_CountryPersonId",
                table: "Persons",
                columns: new[] { "CountryId", "CountryPersonId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PersonContries_Persons_PersonEntityId",
                table: "PersonContries",
                column: "PersonEntityId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonContries_Countries_CountryEntityId",
                table: "PersonContries",
                column: "CountryEntityId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Countries_CountryEntityId",
                table: "Persons",
                column: "CountryEntityId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Countries_Persons_PersonEntityId",
                table: "Countries");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonContries_Persons_PersonEntityId",
                table: "PersonContries");

            migrationBuilder.DropForeignKey(
                name: "FK_Countries_PersonContries_PersonCountryId_PersonId",
                table: "Countries");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "PersonContries");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
