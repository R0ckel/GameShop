using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameShopAPI.Migrations
{
    /// <inheritdoc />
    public partial class WithImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailImagePath",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailImagePath",
                table: "Games",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ThumbnailImagePath",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ThumbnailImagePath",
                table: "Games");
        }
    }
}
