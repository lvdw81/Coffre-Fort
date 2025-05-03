using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace coffre_fort_api.Migrations
{
    /// <inheritdoc />
    public partial class RebuildDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "PasswordEntries");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "MotDePasseHash");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users",
                newName: "Identifiant");

            migrationBuilder.RenameColumn(
                name: "Nom",
                table: "PasswordEntries",
                newName: "NomApplication");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MotDePasseHash",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "Identifiant",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "NomApplication",
                table: "PasswordEntries",
                newName: "Nom");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "PasswordEntries",
                type: "TEXT",
                nullable: true);
        }
    }
}
