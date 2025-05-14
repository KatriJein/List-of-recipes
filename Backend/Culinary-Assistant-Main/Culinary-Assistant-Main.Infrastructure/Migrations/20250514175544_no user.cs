using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Culinary_Assistant_Main.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class nouser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_Users_UserId",
                schema: "MainAppSchema",
                table: "Receipts");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "MainAppSchema");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_UserId",
                schema: "MainAppSchema",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "MainAppSchema",
                table: "Receipts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "MainAppSchema",
                table: "Receipts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "MainAppSchema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    ProfilePictureUrl = table.Column<string>(type: "text", nullable: true),
                    Email_Value = table.Column<string>(type: "text", nullable: true),
                    Login_Value = table.Column<string>(type: "text", nullable: false),
                    Phone_Value = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_UserId",
                schema: "MainAppSchema",
                table: "Receipts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_Users_UserId",
                schema: "MainAppSchema",
                table: "Receipts",
                column: "UserId",
                principalSchema: "MainAppSchema",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
