using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class UpdateExpenseModelPropertiesCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Categories_CategoryId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_SubCategories_SubCategoryId",
                table: "Expenses");

            migrationBuilder.RenameColumn(
                name: "SubCategoryId",
                table: "Expenses",
                newName: "SubCategoryExpenseId");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Expenses",
                newName: "CategoryExpenseId");

            migrationBuilder.RenameIndex(
                name: "IX_Expenses_SubCategoryId",
                table: "Expenses",
                newName: "IX_Expenses_SubCategoryExpenseId");

            migrationBuilder.RenameIndex(
                name: "IX_Expenses_CategoryId",
                table: "Expenses",
                newName: "IX_Expenses_CategoryExpenseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_CategoriesExpense_CategoryExpenseId",
                table: "Expenses",
                column: "CategoryExpenseId",
                principalTable: "CategoriesExpense",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_SubCategoriesExpense_SubCategoryExpenseId",
                table: "Expenses",
                column: "SubCategoryExpenseId",
                principalTable: "SubCategoriesExpense",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_CategoriesExpense_CategoryExpenseId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_SubCategoriesExpense_SubCategoryExpenseId",
                table: "Expenses");

            migrationBuilder.RenameColumn(
                name: "SubCategoryExpenseId",
                table: "Expenses",
                newName: "SubCategoryId");

            migrationBuilder.RenameColumn(
                name: "CategoryExpenseId",
                table: "Expenses",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Expenses_SubCategoryExpenseId",
                table: "Expenses",
                newName: "IX_Expenses_SubCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Expenses_CategoryExpenseId",
                table: "Expenses",
                newName: "IX_Expenses_CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Categories_CategoryId",
                table: "Expenses",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_SubCategories_SubCategoryId",
                table: "Expenses",
                column: "SubCategoryId",
                principalTable: "SubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
