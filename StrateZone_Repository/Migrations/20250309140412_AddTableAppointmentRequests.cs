using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StrateZone_Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddTableAppointmentRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "appointment_requests",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    from_user = table.Column<int>(type: "integer", nullable: false),
                    to_user = table.Column<int>(type: "integer", nullable: false),
                    appointment_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "request_status", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("appointment_requests_pkey", x => x.id);
                    table.ForeignKey(
                        name: "appointment_requests_from_user_fkey",
                        column: x => x.from_user,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "appointment_requests_to_appointment_fkey",
                        column: x => x.appointment_id,
                        principalTable: "appointments",
                        principalColumn: "appointment_id");
                    table.ForeignKey(
                        name: "appointment_requests_to_user_fkey",
                        column: x => x.to_user,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_appointment_requests_appointment_id",
                table: "appointment_requests",
                column: "appointment_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_requests_from_user",
                table: "appointment_requests",
                column: "from_user");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_requests_to_user",
                table: "appointment_requests",
                column: "to_user");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointment_requests");
        }
    }
}
