using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace coffre_fort_api.Migrations
{
    /// <inheritdoc />
    public partial class AddTagsToPasswordEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "PasswordEntries");

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "PasswordEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "PasswordEntries");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "PasswordEntries",
                type: "TEXT",
                nullable: true);
        }
    }
}
