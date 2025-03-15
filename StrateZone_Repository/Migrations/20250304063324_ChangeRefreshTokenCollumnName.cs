using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StrateZone_Repository.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRefreshTokenCollumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
            name: "RefreshToken",   // Old incorrect name
            table: "users",   // Your table name
            newName: "refresh_token"); // Correct name

            migrationBuilder.RenameColumn(
            name: "RefreshTokenExpiry",   // Old incorrect name
            table: "users",   // Your table name
            newName: "refresh_token_expiry"); // Correct name
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
            name: "refresh_token",
            table: "users",
            newName: "RefreshToken");

            migrationBuilder.RenameColumn(
            name: "refresh_token_expiry",
            table: "users",
            newName: "RefreshTokenExpiry");
        }
    }
}
