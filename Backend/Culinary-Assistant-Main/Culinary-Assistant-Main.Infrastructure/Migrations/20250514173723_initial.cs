using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Culinary_Assistant_Main.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "MainAppSchema");

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "MainAppSchema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Login_Value = table.Column<string>(type: "text", nullable: false),
                    Phone_Value = table.Column<long>(type: "bigint", nullable: true),
                    Email_Value = table.Column<string>(type: "text", nullable: true),
                    ProfilePictureUrl = table.Column<string>(type: "text", nullable: true),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Receipts",
                schema: "MainAppSchema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title_Value = table.Column<string>(type: "text", nullable: false),
                    Description_Value = table.Column<string>(type: "text", nullable: false),
                    CookingDifficulty = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: false),
                    CookingTime = table.Column<int>(type: "integer", nullable: false),
                    Ingredients = table.Column<string>(type: "text", nullable: false),
                    CookingSteps = table.Column<string>(type: "text", nullable: false),
                    Nutrients_Calories = table.Column<double>(type: "double precision", nullable: false),
                    Nutrients_Proteins = table.Column<double>(type: "double precision", nullable: false),
                    Nutrients_Fats = table.Column<double>(type: "double precision", nullable: false),
                    Nutrients_Carbohydrates = table.Column<double>(type: "double precision", nullable: false),
                    Popularity = table.Column<int>(type: "integer", nullable: false),
                    MainPictureUrl = table.Column<string>(type: "text", nullable: false),
                    PicturesUrls = table.Column<string>(type: "text", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Receipts_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "MainAppSchema",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_UserId",
                schema: "MainAppSchema",
                table: "Receipts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Receipts",
                schema: "MainAppSchema");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "MainAppSchema");
        }
    }
}
