using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RapidCMS.Example.ModelMaker.Migrations
{
    public partial class Blogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OnetoManyManys_OnetoManyOnes_OneId",
                table: "OnetoManyManys");

            migrationBuilder.DropIndex(
                name: "IX_OnetoManyManys_OneId",
                table: "OnetoManyManys");

            migrationBuilder.DropColumn(
                name: "OneId",
                table: "OnetoManyManys");

            migrationBuilder.AddColumn<int>(
                name: "OneId",
                table: "OnetoManyOnes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Blogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(127)", maxLength: 127, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MainCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blogs_Categories_MainCategoryId",
                        column: x => x.MainCategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BlogCategory",
                columns: table => new
                {
                    BlogBlogCategoriesId = table.Column<int>(type: "int", nullable: false),
                    BlogCategoriesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogCategory", x => new { x.BlogBlogCategoriesId, x.BlogCategoriesId });
                    table.ForeignKey(
                        name: "FK_BlogCategory_Blogs_BlogBlogCategoriesId",
                        column: x => x.BlogBlogCategoriesId,
                        principalTable: "Blogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogCategory_Categories_BlogCategoriesId",
                        column: x => x.BlogCategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OnetoManyOnes_OneId",
                table: "OnetoManyOnes",
                column: "OneId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogCategory_BlogCategoriesId",
                table: "BlogCategory",
                column: "BlogCategoriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_MainCategoryId",
                table: "Blogs",
                column: "MainCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_OnetoManyOnes_OnetoManyManys_OneId",
                table: "OnetoManyOnes",
                column: "OneId",
                principalTable: "OnetoManyManys",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OnetoManyOnes_OnetoManyManys_OneId",
                table: "OnetoManyOnes");

            migrationBuilder.DropTable(
                name: "BlogCategory");

            migrationBuilder.DropTable(
                name: "Blogs");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_OnetoManyOnes_OneId",
                table: "OnetoManyOnes");

            migrationBuilder.DropColumn(
                name: "OneId",
                table: "OnetoManyOnes");

            migrationBuilder.AddColumn<int>(
                name: "OneId",
                table: "OnetoManyManys",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OnetoManyManys_OneId",
                table: "OnetoManyManys",
                column: "OneId");

            migrationBuilder.AddForeignKey(
                name: "FK_OnetoManyManys_OnetoManyOnes_OneId",
                table: "OnetoManyManys",
                column: "OneId",
                principalTable: "OnetoManyOnes",
                principalColumn: "Id");
        }
    }
}
