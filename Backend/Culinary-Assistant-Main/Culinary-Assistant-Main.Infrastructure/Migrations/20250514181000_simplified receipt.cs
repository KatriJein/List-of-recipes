using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Culinary_Assistant_Main.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class simplifiedreceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                schema: "MainAppSchema",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "CookingDifficulty",
                schema: "MainAppSchema",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "CookingSteps",
                schema: "MainAppSchema",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "CookingTime",
                schema: "MainAppSchema",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Ingredients",
                schema: "MainAppSchema",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Popularity",
                schema: "MainAppSchema",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Rating",
                schema: "MainAppSchema",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Tags",
                schema: "MainAppSchema",
                table: "Receipts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Category",
                schema: "MainAppSchema",
                table: "Receipts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CookingDifficulty",
                schema: "MainAppSchema",
                table: "Receipts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CookingSteps",
                schema: "MainAppSchema",
                table: "Receipts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CookingTime",
                schema: "MainAppSchema",
                table: "Receipts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Ingredients",
                schema: "MainAppSchema",
                table: "Receipts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Popularity",
                schema: "MainAppSchema",
                table: "Receipts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                schema: "MainAppSchema",
                table: "Receipts",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                schema: "MainAppSchema",
                table: "Receipts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
