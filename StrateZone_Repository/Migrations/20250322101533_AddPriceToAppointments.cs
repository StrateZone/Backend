using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StrateZone_Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceToAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "friendrequests",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "appointment_requests",
                newName: "status");

            migrationBuilder.AddColumn<decimal>(
                name: "total_price",
                table: "appointments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "total_price",
                table: "appointments");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "friendrequests",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "appointment_requests",
                newName: "Status");
        }
    }
}
