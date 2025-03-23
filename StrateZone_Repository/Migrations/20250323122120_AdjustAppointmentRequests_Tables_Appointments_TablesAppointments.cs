using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StrateZone_Repository.Migrations
{
    /// <inheritdoc />
    public partial class AdjustAppointmentRequests_Tables_Appointments_TablesAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "appointment_requests_to_table_appointment_fkey",
                table: "appointment_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_tables_appointments_gameExtensions_extension_id",
                table: "tables_appointments");

            migrationBuilder.RenameColumn(
                name: "extension_id",
                table: "tables_appointments",
                newName: "GameExtensionExtensionId");

            migrationBuilder.RenameIndex(
                name: "IX_tables_appointments_extension_id",
                table: "tables_appointments",
                newName: "IX_tables_appointments_GameExtensionExtensionId");

            migrationBuilder.RenameColumn(
                name: "table_appointment_id",
                table: "appointment_requests",
                newName: "table_id");

            migrationBuilder.RenameIndex(
                name: "IX_appointment_requests_table_appointment_id",
                table: "appointment_requests",
                newName: "IX_appointment_requests_table_id");

            migrationBuilder.AddColumn<int>(
                name: "TablesAppointmentId",
                table: "appointment_requests",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "appointment_id",
                table: "appointment_requests",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "expire_at",
                table: "appointment_requests",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_appointment_requests_appointment_id",
                table: "appointment_requests",
                column: "appointment_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_requests_TablesAppointmentId",
                table: "appointment_requests",
                column: "TablesAppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_appointment_requests_tables_appointments_TablesAppointmentId",
                table: "appointment_requests",
                column: "TablesAppointmentId",
                principalTable: "tables_appointments",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "appointment_requests_to_appointment_fkey",
                table: "appointment_requests",
                column: "appointment_id",
                principalTable: "appointments",
                principalColumn: "appointment_id");

            migrationBuilder.AddForeignKey(
                name: "appointment_requests_to_table_fkey",
                table: "appointment_requests",
                column: "table_id",
                principalTable: "tables",
                principalColumn: "table_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tables_appointments_gameExtensions_GameExtensionExtensionId",
                table: "tables_appointments",
                column: "GameExtensionExtensionId",
                principalTable: "gameExtensions",
                principalColumn: "extension_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointment_requests_tables_appointments_TablesAppointmentId",
                table: "appointment_requests");

            migrationBuilder.DropForeignKey(
                name: "appointment_requests_to_appointment_fkey",
                table: "appointment_requests");

            migrationBuilder.DropForeignKey(
                name: "appointment_requests_to_table_fkey",
                table: "appointment_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_tables_appointments_gameExtensions_GameExtensionExtensionId",
                table: "tables_appointments");

            migrationBuilder.DropIndex(
                name: "IX_appointment_requests_appointment_id",
                table: "appointment_requests");

            migrationBuilder.DropIndex(
                name: "IX_appointment_requests_TablesAppointmentId",
                table: "appointment_requests");

            migrationBuilder.DropColumn(
                name: "TablesAppointmentId",
                table: "appointment_requests");

            migrationBuilder.DropColumn(
                name: "appointment_id",
                table: "appointment_requests");

            migrationBuilder.DropColumn(
                name: "expire_at",
                table: "appointment_requests");

            migrationBuilder.RenameColumn(
                name: "GameExtensionExtensionId",
                table: "tables_appointments",
                newName: "extension_id");

            migrationBuilder.RenameIndex(
                name: "IX_tables_appointments_GameExtensionExtensionId",
                table: "tables_appointments",
                newName: "IX_tables_appointments_extension_id");

            migrationBuilder.RenameColumn(
                name: "table_id",
                table: "appointment_requests",
                newName: "table_appointment_id");

            migrationBuilder.RenameIndex(
                name: "IX_appointment_requests_table_id",
                table: "appointment_requests",
                newName: "IX_appointment_requests_table_appointment_id");

            migrationBuilder.AddForeignKey(
                name: "appointment_requests_to_table_appointment_fkey",
                table: "appointment_requests",
                column: "table_appointment_id",
                principalTable: "tables_appointments",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_tables_appointments_gameExtensions_extension_id",
                table: "tables_appointments",
                column: "extension_id",
                principalTable: "gameExtensions",
                principalColumn: "extension_id");
        }
    }
}
