using Microsoft.EntityFrameworkCore.Migrations;

namespace RapidCMS.Example.ModelMaker.Migrations
{
    public partial class RenamedEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blog_Category_MainCategoryId",
                table: "Blog");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogCategory_Blog_BlogCategoriesId",
                table: "BlogCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogCategory_Category_CategoriesId",
                table: "BlogCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Blog",
                table: "Blog");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "Categories");

            migrationBuilder.RenameTable(
                name: "Blog",
                newName: "Blogs");

            migrationBuilder.RenameIndex(
                name: "IX_Blog_MainCategoryId",
                table: "Blogs",
                newName: "IX_Blogs_MainCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Blogs",
                table: "Blogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogCategory_Blogs_BlogCategoriesId",
                table: "BlogCategory",
                column: "BlogCategoriesId",
                principalTable: "Blogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogCategory_Categories_CategoriesId",
                table: "BlogCategory",
                column: "CategoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Categories_MainCategoryId",
                table: "Blogs",
                column: "MainCategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogCategory_Blogs_BlogCategoriesId",
                table: "BlogCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogCategory_Categories_CategoriesId",
                table: "BlogCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Categories_MainCategoryId",
                table: "Blogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Blogs",
                table: "Blogs");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Category");

            migrationBuilder.RenameTable(
                name: "Blogs",
                newName: "Blog");

            migrationBuilder.RenameIndex(
                name: "IX_Blogs_MainCategoryId",
                table: "Blog",
                newName: "IX_Blog_MainCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Blog",
                table: "Blog",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Blog_Category_MainCategoryId",
                table: "Blog",
                column: "MainCategoryId",
                principalTable: "Category",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogCategory_Blog_BlogCategoriesId",
                table: "BlogCategory",
                column: "BlogCategoriesId",
                principalTable: "Blog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogCategory_Category_CategoriesId",
                table: "BlogCategory",
                column: "CategoriesId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
