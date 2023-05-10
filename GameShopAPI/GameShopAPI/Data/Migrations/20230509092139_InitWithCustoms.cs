﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameShopAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitWithCustoms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comments_UserId",
                table: "Comments");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId_GameId_Id",
                table: "Comments",
                columns: new[] { "UserId", "GameId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BasketItems_UserId_GameId",
                table: "BasketItems",
                columns: new[] { "UserId", "GameId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Comments_UserId_GameId_Id",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_BasketItems_UserId_GameId",
                table: "BasketItems");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");
        }
    }
}
