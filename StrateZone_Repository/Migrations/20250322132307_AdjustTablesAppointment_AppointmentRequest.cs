using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StrateZone_Repository.Migrations
{
    /// <inheritdoc />
    public partial class AdjustTablesAppointment_AppointmentRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "appointment_requests_to_appointment_fkey",
                table: "appointment_requests");

            migrationBuilder.RenameColumn(
                name: "appointment_id",
                table: "appointment_requests",
                newName: "table_appointment_id");

            migrationBuilder.RenameIndex(
                name: "IX_appointment_requests_appointment_id",
                table: "appointment_requests",
                newName: "IX_appointment_requests_table_appointment_id");

            migrationBuilder.AddForeignKey(
                name: "appointment_requests_to_table_appointment_fkey",
                table: "appointment_requests",
                column: "table_appointment_id",
                principalTable: "tables_appointments",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "appointment_requests_to_table_appointment_fkey",
                table: "appointment_requests");

            migrationBuilder.RenameColumn(
                name: "table_appointment_id",
                table: "appointment_requests",
                newName: "appointment_id");

            migrationBuilder.RenameIndex(
                name: "IX_appointment_requests_table_appointment_id",
                table: "appointment_requests",
                newName: "IX_appointment_requests_appointment_id");

            migrationBuilder.AddForeignKey(
                name: "appointment_requests_to_appointment_fkey",
                table: "appointment_requests",
                column: "appointment_id",
                principalTable: "appointments",
                principalColumn: "appointment_id");
        }
    }
}
