using Microsoft.EntityFrameworkCore.Migrations;
using StrateZone_Repository.Parameters;

#nullable disable

namespace StrateZone_Repository.Migrations
{
    /// <inheritdoc />
    public partial class Fix_GameTypes_GameExtensions_Tables_Appointments_Relationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "tables_gameExtension_id_fkey",
                table: "tables");

            migrationBuilder.RenameColumn(
                name: "gameExtension_id",
                table: "tables",
                newName: "gameType_id");

            migrationBuilder.RenameIndex(
                name: "IX_tables_gameExtension_id",
                table: "tables",
                newName: "IX_tables_gameType_id");

            migrationBuilder.AddColumn<int>(
                name: "extension_id",
                table: "tables_appointments",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tables_appointments_extension_id",
                table: "tables_appointments",
                column: "extension_id");

            migrationBuilder.AddForeignKey(
                name: "tables_gameType_id_fkey",
                table: "tables",
                column: "gameType_id",
                principalTable: "gameTypes",
                principalColumn: "type_id");

            migrationBuilder.AddForeignKey(
                name: "tables_appointments_extension_id_fkey",
                table: "tables_appointments",
                column: "extension_id",
                principalTable: "gameExtensions",
                principalColumn: "extension_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "tables_gameType_id_fkey",
                table: "tables");

            migrationBuilder.DropForeignKey(
                name: "tables_appointments_extension_id_fkey",
                table: "tables_appointments");

            migrationBuilder.DropIndex(
                name: "IX_tables_appointments_extension_id",
                table: "tables_appointments");

            migrationBuilder.DropColumn(
                name: "extension_id",
                table: "tables_appointments");

            migrationBuilder.RenameColumn(
                name: "gameType_id",
                table: "tables",
                newName: "gameExtension_id");

            migrationBuilder.RenameIndex(
                name: "IX_tables_gameType_id",
                table: "tables",
                newName: "IX_tables_gameExtension_id");

            migrationBuilder.AddForeignKey(
                name: "tables_gameExtension_id_fkey",
                table: "tables",
                column: "gameExtension_id",
                principalTable: "gameExtensions",
                principalColumn: "extension_id");
        }
    }
}
