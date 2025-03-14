using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StrateZone_Repository.Migrations
{
    /// <inheritdoc />
    public partial class Update_RoomsPrices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "tables_appointments_extension_id_fkey",
                table: "tables_appointments");

            /*
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
                .OldAnnotation("Npgsql:Enum:product_status", "available,out_of_stock,discontinued")
                .OldAnnotation("Npgsql:Enum:ranking", "basic,silver,gold,platinum")
                .OldAnnotation("Npgsql:Enum:request_status", "pending,accepted,rejected,cancelled")
                .OldAnnotation("Npgsql:Enum:room_status", "available,unavailable,closed")
                .OldAnnotation("Npgsql:Enum:room_type", "study,appointment")
                .OldAnnotation("Npgsql:Enum:skill_level", "beginner,intermediate,advanced")
                .OldAnnotation("Npgsql:Enum:thread_status", "published,rejected,pending,deleted")
                .OldAnnotation("Npgsql:Enum:ticket_type", "withdrawal,feedback,other")
                .OldAnnotation("Npgsql:Enum:transaction_type", "deposit,withdrawal,refund")
                .OldAnnotation("Npgsql:Enum:user_course_result", "passed,failed")
                .OldAnnotation("Npgsql:Enum:user_role", "registered_user,member,instructor,staff,admin")
                .OldAnnotation("Npgsql:Enum:voucher_status", "active,expired")
                .OldAnnotation("Npgsql:Enum:wallet_status", "active,closed");
            */

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "rooms",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "room_id",
                table: "prices",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_prices_room_id",
                table: "prices",
                column: "room_id");

            migrationBuilder.AddForeignKey(
                name: "prices_room_id_fkey",
                table: "prices",
                column: "room_id",
                principalTable: "rooms",
                principalColumn: "room_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tables_appointments_gameExtensions_extension_id",
                table: "tables_appointments",
                column: "extension_id",
                principalTable: "gameExtensions",
                principalColumn: "extension_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "prices_room_id_fkey",
                table: "prices");

            migrationBuilder.DropForeignKey(
                name: "FK_tables_appointments_gameExtensions_extension_id",
                table: "tables_appointments");

            migrationBuilder.DropIndex(
                name: "IX_prices_room_id",
                table: "prices");

            migrationBuilder.DropColumn(
                name: "description",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "room_id",
                table: "prices");
            
            /*
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
                .Annotation("Npgsql:Enum:product_status", "available,out_of_stock,discontinued")
                .Annotation("Npgsql:Enum:ranking", "basic,silver,gold,platinum")
                .Annotation("Npgsql:Enum:request_status", "pending,accepted,rejected,cancelled")
                .Annotation("Npgsql:Enum:room_status", "available,unavailable,closed")
                .Annotation("Npgsql:Enum:room_type", "study,appointment")
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
            */

            migrationBuilder.AddForeignKey(
                name: "tables_appointments_extension_id_fkey",
                table: "tables_appointments",
                column: "extension_id",
                principalTable: "gameExtensions",
                principalColumn: "extension_id");
        }
    }
}
