using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StrateZone_Repository.Migrations
{
    /// <inheritdoc />
    public partial class FixEnums_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:course_slot_status.course_slot_status", "upcoming,in_progress,completed,cancelled")
                .Annotation("Npgsql:Enum:course_status.course_status", "open,closed,in_progress,completed,cancelled")
                .Annotation("Npgsql:Enum:event_status.event_status", "upcoming,ongoing,completed,cancelled")
                .Annotation("Npgsql:Enum:event_type.event_type", "tournament,promotion")
                .Annotation("Npgsql:Enum:game_extension.game_extension", "bullet,lightning,flip,traditional")
                .Annotation("Npgsql:Enum:game_type.game_type", "chess,xiangqi,go")
                .Annotation("Npgsql:Enum:gender.gender", "male,female")
                .Annotation("Npgsql:Enum:message_status.message_status", "read,unread")
                .Annotation("Npgsql:Enum:order_status.order_status", "pending,shipped,delivered,cancelled")
                .Annotation("Npgsql:Enum:participant_status.participant_status", "enrolled,drop_out,in_progress,completed")
                .Annotation("Npgsql:Enum:product_status.product_status", "available,out_of_stock,discontinued")
                .Annotation("Npgsql:Enum:ranking.ranking", "basic,silver,gold,platinum")
                .Annotation("Npgsql:Enum:request_status.request_status", "pending,accepted,rejected,cancelled")
                .Annotation("Npgsql:Enum:room_status.room_status", "available,unavailable,closed")
                .Annotation("Npgsql:Enum:room_type.room_type", "study,appointment")
                .Annotation("Npgsql:Enum:skill_level.skill_level", "beginner,intermediate,advanced")
                .Annotation("Npgsql:Enum:thread_status.thread_status", "published,rejected,pending,deleted")
                .Annotation("Npgsql:Enum:ticket_type.ticket_type", "withdrawal,feedback,other")
                .Annotation("Npgsql:Enum:transaction_type.transaction_type", "deposit,withdrawal,refund")
                .Annotation("Npgsql:Enum:user_course_result.user_course_result", "passed,failed")
                .Annotation("Npgsql:Enum:user_role.user_role", "registered_user,member,instructor,staff,admin")
                .Annotation("Npgsql:Enum:voucher_status.voucher_status", "active,expired")
                .Annotation("Npgsql:Enum:wallet_status.wallet_status", "active,closed")
                .OldAnnotation("Npgsql:Enum:course_slot_status", "upcoming,in_progress,completed,cancelled")
                .OldAnnotation("Npgsql:Enum:course_status", "open,closed,in_progress,completed,cancelled")
                .OldAnnotation("Npgsql:Enum:event_status", "upcoming,ongoing,completed,cancelled")
                .OldAnnotation("Npgsql:Enum:event_type", "tournament,promotion")
                .OldAnnotation("Npgsql:Enum:game_extension", "bullet,lightning,flip,traditional")
                .OldAnnotation("Npgsql:Enum:game_type", "chess,xiangqi,go")
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

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "wallet",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "vouchers",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "result",
                table: "users_courses",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "role",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "gender",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "transactions",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "ticket_type",
                table: "tickets",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "threads",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "rooms",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "room_type",
                table: "rooms",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "products",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "orders",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "messages",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "events",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "events",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "courses_slot",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "courses",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "skill_level",
                table: "courses",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "chess_type",
                table: "courses",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "appointments",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:course_slot_status", "upcoming,in_progress,completed,cancelled")
                .Annotation("Npgsql:Enum:course_status", "open,closed,in_progress,completed,cancelled")
                .Annotation("Npgsql:Enum:event_status", "upcoming,ongoing,completed,cancelled")
                .Annotation("Npgsql:Enum:event_type", "tournament,promotion")
                .Annotation("Npgsql:Enum:game_extension", "bullet,lightning,flip,traditional")
                .Annotation("Npgsql:Enum:game_type", "chess,xiangqi,go")
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
                .OldAnnotation("Npgsql:Enum:course_slot_status.course_slot_status", "upcoming,in_progress,completed,cancelled")
                .OldAnnotation("Npgsql:Enum:course_status.course_status", "open,closed,in_progress,completed,cancelled")
                .OldAnnotation("Npgsql:Enum:event_status.event_status", "upcoming,ongoing,completed,cancelled")
                .OldAnnotation("Npgsql:Enum:event_type.event_type", "tournament,promotion")
                .OldAnnotation("Npgsql:Enum:game_extension.game_extension", "bullet,lightning,flip,traditional")
                .OldAnnotation("Npgsql:Enum:game_type.game_type", "chess,xiangqi,go")
                .OldAnnotation("Npgsql:Enum:gender.gender", "male,female")
                .OldAnnotation("Npgsql:Enum:message_status.message_status", "read,unread")
                .OldAnnotation("Npgsql:Enum:order_status.order_status", "pending,shipped,delivered,cancelled")
                .OldAnnotation("Npgsql:Enum:participant_status.participant_status", "enrolled,drop_out,in_progress,completed")
                .OldAnnotation("Npgsql:Enum:product_status.product_status", "available,out_of_stock,discontinued")
                .OldAnnotation("Npgsql:Enum:ranking.ranking", "basic,silver,gold,platinum")
                .OldAnnotation("Npgsql:Enum:request_status.request_status", "pending,accepted,rejected,cancelled")
                .OldAnnotation("Npgsql:Enum:room_status.room_status", "available,unavailable,closed")
                .OldAnnotation("Npgsql:Enum:room_type.room_type", "study,appointment")
                .OldAnnotation("Npgsql:Enum:skill_level.skill_level", "beginner,intermediate,advanced")
                .OldAnnotation("Npgsql:Enum:thread_status.thread_status", "published,rejected,pending,deleted")
                .OldAnnotation("Npgsql:Enum:ticket_type.ticket_type", "withdrawal,feedback,other")
                .OldAnnotation("Npgsql:Enum:transaction_type.transaction_type", "deposit,withdrawal,refund")
                .OldAnnotation("Npgsql:Enum:user_course_result.user_course_result", "passed,failed")
                .OldAnnotation("Npgsql:Enum:user_role.user_role", "registered_user,member,instructor,staff,admin")
                .OldAnnotation("Npgsql:Enum:voucher_status.voucher_status", "active,expired")
                .OldAnnotation("Npgsql:Enum:wallet_status.wallet_status", "active,closed");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "wallet",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "vouchers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "result",
                table: "users_courses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "role",
                table: "users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "gender",
                table: "users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "type",
                table: "transactions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "ticket_type",
                table: "tickets",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "threads",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "rooms",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "room_type",
                table: "rooms",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "products",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "orders",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "messages",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "type",
                table: "events",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "events",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "courses_slot",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "courses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "skill_level",
                table: "courses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "chess_type",
                table: "courses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "appointments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
