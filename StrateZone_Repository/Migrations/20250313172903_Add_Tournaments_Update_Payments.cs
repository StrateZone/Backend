using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StrateZone_Repository.Migrations
{
    /// <inheritdoc />
    public partial class Add_Tournaments_Update_Payments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "type",
                table: "events");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:appointment_status", "pending,confirmed,acncelled,completed,expired")
                .Annotation("Npgsql:Enum:course_slot_status", "upcoming,in_progress,completed,cancelled")
                .Annotation("Npgsql:Enum:course_status", "open,closed,in_progress,completed,cancelled")
                .Annotation("Npgsql:Enum:event_status", "upcoming,ongoing,completed,cancelled")
                .Annotation("Npgsql:Enum:game_extension", "bullet,flip,lightning,traditional")
                .Annotation("Npgsql:Enum:game_extension_enum", "bullet,lightning,flip,traditional")
                .Annotation("Npgsql:Enum:game_type", "chess,go,xiangqi")
                .Annotation("Npgsql:Enum:game_type_enum", "chess,xiangqi,go")
                .Annotation("Npgsql:Enum:gender", "male,female")
                .Annotation("Npgsql:Enum:message_status", "read,unread")
                .Annotation("Npgsql:Enum:order_status", "pending,shipped,delivered,cancelled")
                .Annotation("Npgsql:Enum:participant_status", "enrolled,drop_out,in_progress,completed")
                .Annotation("Npgsql:Enum:payment_type", "order,appointment,course,membership")
                .Annotation("Npgsql:Enum:product_status", "available,out_of_stock,discontinued")
                .Annotation("Npgsql:Enum:ranking", "basic,silver,gold,platinum")
                .Annotation("Npgsql:Enum:request_status", "pending,accepted,rejected,cancelled")
                .Annotation("Npgsql:Enum:room_status", "available,unavailable,closed")
                .Annotation("Npgsql:Enum:room_type", "study,premium,basic,openspaced")
                .Annotation("Npgsql:Enum:skill_level", "beginner,intermediate,advanced")
                .Annotation("Npgsql:Enum:thread_status", "published,rejected,pending,deleted")
                .Annotation("Npgsql:Enum:ticket_type", "withdrawal,feedback,other")
                .Annotation("Npgsql:Enum:transaction_type", "deposit,withdrawal,refund")
                .Annotation("Npgsql:Enum:user_course_result", "passed,failed")
                .Annotation("Npgsql:Enum:user_role", "registered_user,member,instructor,staff,admin")
                .Annotation("Npgsql:Enum:voucher_status", "active,expired")
                .Annotation("Npgsql:Enum:wallet_status", "active,closed")
                .OldAnnotation("Npgsql:Enum:appointment_status", "pending,confirmed,acncelled,completed,expired")
                .OldAnnotation("Npgsql:Enum:course_slot_status", "upcoming,in_progress,completed,cancelled")
                .OldAnnotation("Npgsql:Enum:course_status", "open,closed,in_progress,completed,cancelled")
                .OldAnnotation("Npgsql:Enum:event_status", "upcoming,ongoing,completed,cancelled")
                .OldAnnotation("Npgsql:Enum:event_type", "tournament,promotion")
                .OldAnnotation("Npgsql:Enum:game_extension", "bullet,flip,lightning,traditional")
                .OldAnnotation("Npgsql:Enum:game_extension_enum", "bullet,lightning,flip,traditional")
                .OldAnnotation("Npgsql:Enum:game_type", "chess,go,xiangqi")
                .OldAnnotation("Npgsql:Enum:game_type_enum", "chess,xiangqi,go")
                .OldAnnotation("Npgsql:Enum:gender", "male,female")
                .OldAnnotation("Npgsql:Enum:message_status", "read,unread")
                .OldAnnotation("Npgsql:Enum:order_status", "pending,shipped,delivered,cancelled")
                .OldAnnotation("Npgsql:Enum:participant_status", "enrolled,drop_out,in_progress,completed")
                .OldAnnotation("Npgsql:Enum:payment_type", "order,appointment,course,membership")
                .OldAnnotation("Npgsql:Enum:product_status", "available,out_of_stock,discontinued")
                .OldAnnotation("Npgsql:Enum:ranking", "basic,silver,gold,platinum")
                .OldAnnotation("Npgsql:Enum:request_status", "pending,accepted,rejected,cancelled")
                .OldAnnotation("Npgsql:Enum:room_status", "available,unavailable,closed")
                .OldAnnotation("Npgsql:Enum:room_type", "study,premium,basic,openspaced")
                .OldAnnotation("Npgsql:Enum:skill_level", "beginner,intermediate,advanced")
                .OldAnnotation("Npgsql:Enum:thread_status", "published,rejected,pending,deleted")
                .OldAnnotation("Npgsql:Enum:ticket_type", "withdrawal,feedback,other")
                .OldAnnotation("Npgsql:Enum:transaction_type", "deposit,withdrawal,refund")
                .OldAnnotation("Npgsql:Enum:user_course_result", "passed,failed")
                .OldAnnotation("Npgsql:Enum:user_role", "registered_user,member,instructor,staff,admin")
                .OldAnnotation("Npgsql:Enum:voucher_status", "active,expired")
                .OldAnnotation("Npgsql:Enum:wallet_status", "active,closed");

            migrationBuilder.AddColumn<int>(
                name: "appointment_id",
                table: "payments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "course_id",
                table: "payments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payment_type",
                table: "payments",
                type: "text",
                nullable: false,
                defaultValue: ""
                );

            migrationBuilder.AddColumn<string>(
                name: "ranking",
                table: "users",
                type: "ranking",
                nullable: false,
                defaultValue: "basic"
            );

            migrationBuilder.AddColumn<short>(
                name: "NumberOfPlayers",
                table: "gameExtensions",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "tournaments",
                columns: table => new
                {
                    tournament_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    targeted_ranking = table.Column<string>(type: "text", nullable: false),
                    max_participants = table.Column<int>(type: "integer", nullable: true),
                    start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tournaments_pkey", x => x.tournament_id);
                    table.ForeignKey(
                        name: "tournaments_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "tournaments_participants",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tournament_id = table.Column<int>(type: "integer", nullable: true),
                    participant_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tournaments_participants_pkey", x => x.id);
                    table.ForeignKey(
                        name: "tournament_participants_participant_id_fkey",
                        column: x => x.participant_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "tournament_participants_tournament_id_fkey",
                        column: x => x.tournament_id,
                        principalTable: "tournaments",
                        principalColumn: "tournament_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_user_id",
                table: "tournaments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_participants_participant_id",
                table: "tournaments_participants",
                column: "participant_id");

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_participants_tournament_id",
                table: "tournaments_participants",
                column: "tournament_id");

            migrationBuilder.AddForeignKey(
                name: "payments_appointment_id_fkey",
                table: "payments",
                column: "user_id",
                principalTable: "appointments",
                principalColumn: "appointment_id");

            migrationBuilder.AddForeignKey(
                name: "payments_course_id_fkey",
                table: "payments",
                column: "user_id",
                principalTable: "courses",
                principalColumn: "course_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "payments_appointment_id_fkey",
                table: "payments");

            migrationBuilder.DropForeignKey(
                name: "payments_course_id_fkey",
                table: "payments");

            migrationBuilder.DropTable(
                name: "tournaments_participants");

            migrationBuilder.DropTable(
                name: "tournaments");

            migrationBuilder.DropColumn(
                name: "appointment_id",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "course_id",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "payment_type",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "NumberOfPlayers",
                table: "gameExtensions");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:appointment_status", "pending,confirmed,acncelled,completed,expired")
                .Annotation("Npgsql:Enum:course_slot_status", "upcoming,in_progress,completed,cancelled")
                .Annotation("Npgsql:Enum:course_status", "open,closed,in_progress,completed,cancelled")
                .Annotation("Npgsql:Enum:event_status", "upcoming,ongoing,completed,cancelled")
                .Annotation("Npgsql:Enum:event_type", "tournament,promotion")
                .Annotation("Npgsql:Enum:game_extension", "bullet,flip,lightning,traditional")
                .Annotation("Npgsql:Enum:game_extension_enum", "bullet,lightning,flip,traditional")
                .Annotation("Npgsql:Enum:game_type", "chess,go,xiangqi")
                .Annotation("Npgsql:Enum:game_type_enum", "chess,xiangqi,go")
                .Annotation("Npgsql:Enum:gender", "male,female")
                .Annotation("Npgsql:Enum:message_status", "read,unread")
                .Annotation("Npgsql:Enum:order_status", "pending,shipped,delivered,cancelled")
                .Annotation("Npgsql:Enum:participant_status", "enrolled,drop_out,in_progress,completed")
                .Annotation("Npgsql:Enum:payment_type", "order,appointment,course,membership")
                .Annotation("Npgsql:Enum:product_status", "available,out_of_stock,discontinued")
                .Annotation("Npgsql:Enum:ranking", "basic,silver,gold,platinum")
                .Annotation("Npgsql:Enum:request_status", "pending,accepted,rejected,cancelled")
                .Annotation("Npgsql:Enum:room_status", "available,unavailable,closed")
                .Annotation("Npgsql:Enum:room_type", "study,premium,basic,openspaced")
                .Annotation("Npgsql:Enum:skill_level", "beginner,intermediate,advanced")
                .Annotation("Npgsql:Enum:thread_status", "published,rejected,pending,deleted")
                .Annotation("Npgsql:Enum:ticket_type", "withdrawal,feedback,other")
                .Annotation("Npgsql:Enum:transaction_type", "deposit,withdrawal,refund")
                .Annotation("Npgsql:Enum:user_course_result", "passed,failed")
                .Annotation("Npgsql:Enum:user_role", "registered_user,member,instructor,staff,admin")
                .Annotation("Npgsql:Enum:voucher_status", "active,expired")
                .Annotation("Npgsql:Enum:wallet_status", "active,closed")
                .OldAnnotation("Npgsql:Enum:appointment_status", "pending,confirmed,cancelled,completed,expired")
                .OldAnnotation("Npgsql:Enum:course_slot_status", "upcoming,in_progress,completed,cancelled")
                .OldAnnotation("Npgsql:Enum:course_status", "open,closed,in_progress,completed,cancelled")
                .OldAnnotation("Npgsql:Enum:event_status", "upcoming,ongoing,completed,cancelled")
                .OldAnnotation("Npgsql:Enum:game_extension", "bullet,flip,lightning,traditional")
                .OldAnnotation("Npgsql:Enum:game_extension_enum", "bullet,lightning,flip,traditional")
                .OldAnnotation("Npgsql:Enum:game_type", "chess,go,xiangqi")
                .OldAnnotation("Npgsql:Enum:game_type_enum", "chess,xiangqi,go")
                .OldAnnotation("Npgsql:Enum:gender", "male,female")
                .OldAnnotation("Npgsql:Enum:message_status", "read,unread")
                .OldAnnotation("Npgsql:Enum:order_status", "pending,shipped,delivered,cancelled")
                .OldAnnotation("Npgsql:Enum:participant_status", "enrolled,drop_out,in_progress,completed")
                .OldAnnotation("Npgsql:Enum:payment_type", "order,appointment,course,membership")
                .OldAnnotation("Npgsql:Enum:product_status", "available,out_of_stock,discontinued")
                .OldAnnotation("Npgsql:Enum:ranking", "basic,silver,gold,platinum")
                .OldAnnotation("Npgsql:Enum:request_status", "pending,accepted,rejected,cancelled")
                .OldAnnotation("Npgsql:Enum:room_status", "available,unavailable,closed")
                .OldAnnotation("Npgsql:Enum:room_type", "study,premium,basic,openspaced")
                .OldAnnotation("Npgsql:Enum:skill_level", "beginner,intermediate,advanced")
                .OldAnnotation("Npgsql:Enum:thread_status", "published,rejected,pending,deleted")
                .OldAnnotation("Npgsql:Enum:ticket_type", "withdrawal,feedback,other")
                .OldAnnotation("Npgsql:Enum:transaction_type", "deposit,withdrawal,refund")
                .OldAnnotation("Npgsql:Enum:user_course_result", "passed,failed")
                .OldAnnotation("Npgsql:Enum:user_role", "registered_user,member,instructor,staff,admin")
                .OldAnnotation("Npgsql:Enum:voucher_status", "active,expired")
                .OldAnnotation("Npgsql:Enum:wallet_status", "active,closed");

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "events",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
