using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RapidCMS.Example.ModelMaker.Models.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

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
                name: "OnetoManyManys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnetoManyManys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OnetoOneOneBs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnetoOneOneBs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Blogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MainCategoryId = table.Column<int>(type: "int", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "OnetoManyOnes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OneId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnetoManyOnes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnetoManyOnes_OnetoManyManys_OneId",
                        column: x => x.OneId,
                        principalTable: "OnetoManyManys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OnetoOneOneAs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnetoOneOneAs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnetoOneOneAs_OnetoOneOneBs_BId",
                        column: x => x.BId,
                        principalTable: "OnetoOneOneBs",
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
                name: "IX_BlogCategory_BlogCategoriesId",
                table: "BlogCategory",
                column: "BlogCategoriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_MainCategoryId",
                table: "Blogs",
                column: "MainCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ManytoManyManyAManytoManyManyB_BsId",
                table: "ManytoManyManyAManytoManyManyB",
                column: "BsId");

            migrationBuilder.CreateIndex(
                name: "IX_OnetoManyOnes_OneId",
                table: "OnetoManyOnes",
                column: "OneId");

            migrationBuilder.CreateIndex(
                name: "IX_OnetoOneOneAs_BId",
                table: "OnetoOneOneAs",
                column: "BId",
                unique: true,
                filter: "[BId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogCategory");

            migrationBuilder.DropTable(
                name: "ManytoManyManyAManytoManyManyB");

            migrationBuilder.DropTable(
                name: "OnetoManyOnes");

            migrationBuilder.DropTable(
                name: "OnetoOneOneAs");

            migrationBuilder.DropTable(
                name: "Blogs");

            migrationBuilder.DropTable(
                name: "ManytoManyManyAs");

            migrationBuilder.DropTable(
                name: "ManytoManyManyBs");

            migrationBuilder.DropTable(
                name: "OnetoManyManys");

            migrationBuilder.DropTable(
                name: "OnetoOneOneBs");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
