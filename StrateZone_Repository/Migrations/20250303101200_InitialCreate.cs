using System;
using System.Collections;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StrateZone_Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE TYPE course_slot_status AS ENUM ('upcoming', 'in_progress', 'completed', 'cancelled');");
            migrationBuilder.Sql("CREATE TYPE course_status AS ENUM ('open', 'closed', 'in_progress', 'completed', 'cancelled');");
            migrationBuilder.Sql("CREATE TYPE event_status AS ENUM ('upcoming', 'ongoing', 'completed', 'cancelled');");
            migrationBuilder.Sql("CREATE TYPE event_type AS ENUM ('tournament', 'promotion');");
            migrationBuilder.Sql("CREATE TYPE game_extension AS ENUM ('bullet', 'lightning', 'flip', 'traditional');");
            migrationBuilder.Sql("CREATE TYPE game_type AS ENUM ('chess', 'xiangqi', 'go');");
            migrationBuilder.Sql("CREATE TYPE gender AS ENUM ('male', 'female');");
            migrationBuilder.Sql("CREATE TYPE message_status AS ENUM ('read', 'unread');");
            migrationBuilder.Sql("CREATE TYPE order_status AS ENUM ('pending', 'shipped', 'delivered', 'cancelled');");
            migrationBuilder.Sql("CREATE TYPE participant_status AS ENUM ('enrolled', 'drop_out', 'in_progress', 'completed');");
            migrationBuilder.Sql("CREATE TYPE product_status AS ENUM ('available', 'out_of_stock', 'discontinued');");
            migrationBuilder.Sql("CREATE TYPE ranking AS ENUM ('basic', 'silver', 'gold', 'platinum');");
            migrationBuilder.Sql("CREATE TYPE request_status AS ENUM ('pending', 'accepted', 'rejected', 'cancelled');");
            migrationBuilder.Sql("CREATE TYPE room_status AS ENUM ('available', 'unavailable', 'closed');");
            migrationBuilder.Sql("CREATE TYPE room_type AS ENUM ('study', 'appointment');");
            migrationBuilder.Sql("CREATE TYPE skill_level AS ENUM ('beginner', 'intermediate', 'advanced');");
            migrationBuilder.Sql("CREATE TYPE thread_status AS ENUM ('published', 'rejected', 'pending', 'deleted');");
            migrationBuilder.Sql("CREATE TYPE ticket_type AS ENUM ('withdrawal', 'feedback', 'other');");
            migrationBuilder.Sql("CREATE TYPE transaction_type AS ENUM ('deposit', 'withdrawal', 'refund');");
            migrationBuilder.Sql("CREATE TYPE user_course_result AS ENUM ('passed', 'failed');");
            migrationBuilder.Sql("CREATE TYPE user_role AS ENUM ('registered_user', 'member', 'instructor', 'staff', 'admin');");
            migrationBuilder.Sql("CREATE TYPE voucher_status AS ENUM ('active', 'expired');");
            migrationBuilder.Sql("CREATE TYPE wallet_status AS ENUM ('active', 'closed');");

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
                .Annotation("Npgsql:Enum:wallet_status.wallet_status", "active,closed");

            migrationBuilder.CreateTable(
                name: "carts",
                columns: table => new
                {
                    cart_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("carts_pkey", x => x.cart_id);
                });

            migrationBuilder.CreateTable(
                name: "gameTypes",
                columns: table => new
                {
                    type_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type_name = table.Column<string>(type: "game_type", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("gameTypes_pkey", x => x.type_id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    product_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    inventory_count = table.Column<int>(type: "integer", nullable: true),
                    image_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("products_pkey", x => x.product_id);
                });

            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    room_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    room_name = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    room_type = table.Column<int>(type: "integer", nullable: false),
                    capacity = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("rooms_pkey", x => x.room_id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    tag_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tag_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tags_pkey", x => x.tag_id);
                });

            migrationBuilder.CreateTable(
                name: "vouchers",
                columns: table => new
                {
                    voucher_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    voucher_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    min_price_condition = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true, defaultValueSql: "0"),
                    expire_date = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("vouchers_pkey", x => x.voucher_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cart_id = table.Column<int>(type: "integer", nullable: true),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    role = table.Column<string>(type: "user_role", nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "Depends on Role"),
                    gender = table.Column<string>(type: "gender", nullable: false),
                    address = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    avatar_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    bio = table.Column<string>(type: "text", nullable: true),
                    points = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.user_id);
                    table.ForeignKey(
                        name: "users_cart_id_fkey",
                        column: x => x.cart_id,
                        principalTable: "carts",
                        principalColumn: "cart_id");
                });

            migrationBuilder.CreateTable(
                name: "gameExtensions",
                columns: table => new
                {
                    extension_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type_id = table.Column<int>(type: "integer", nullable: true),
                    extension_name = table.Column<string>(type: "game_extension", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("gameExtensions_pkey", x => x.extension_id);
                    table.ForeignKey(
                        name: "gameExtensions_type_id_fkey",
                        column: x => x.type_id,
                        principalTable: "gameTypes",
                        principalColumn: "type_id");
                });

            migrationBuilder.CreateTable(
                name: "cart_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cart_id = table.Column<int>(type: "integer", nullable: true),
                    product_id = table.Column<int>(type: "integer", nullable: true),
                    product_quantity = table.Column<int>(type: "integer", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("cart_items_pkey", x => x.id);
                    table.ForeignKey(
                        name: "cart_items_cart_id_fkey",
                        column: x => x.cart_id,
                        principalTable: "carts",
                        principalColumn: "cart_id");
                    table.ForeignKey(
                        name: "cart_items_product_id_fkey",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id");
                });

            migrationBuilder.CreateTable(
                name: "product_tags",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    product_id = table.Column<int>(type: "integer", nullable: true),
                    tag_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("product_tags_pkey", x => x.id);
                    table.ForeignKey(
                        name: "product_tags_product_id_fkey",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id");
                    table.ForeignKey(
                        name: "product_tags_tag_id_fkey",
                        column: x => x.tag_id,
                        principalTable: "tags",
                        principalColumn: "tag_id");
                });

            migrationBuilder.CreateTable(
                name: "appointments",
                columns: table => new
                {
                    appointment_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    schedule_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("appointments_pkey", x => x.appointment_id);
                    table.ForeignKey(
                        name: "appointments_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "courses",
                columns: table => new
                {
                    course_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    course_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    instructor_id = table.Column<int>(type: "integer", nullable: true),
                    chess_type = table.Column<string>(type: "text", nullable: false),
                    skill_level = table.Column<string>(type: "text", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    max_participants = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("courses_pkey", x => x.course_id);
                    table.ForeignKey(
                        name: "courses_instructor_id_fkey",
                        column: x => x.instructor_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    event_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("events_pkey", x => x.event_id);
                    table.ForeignKey(
                        name: "events_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "friendlists",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    friend_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("friendlists_pkey", x => x.id);
                    table.ForeignKey(
                        name: "friendlists_friend_id_fkey",
                        column: x => x.friend_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "friendlists_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "friendrequests",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    from_user = table.Column<int>(type: "integer", nullable: false),
                    to_user = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "request_status", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("friendrequests_pkey", x => x.id);
                    table.ForeignKey(
                        name: "friendrequests_from_user_fkey",
                        column: x => x.from_user,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "friendrequests_to_user_fkey",
                        column: x => x.to_user,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    message_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sender_id = table.Column<int>(type: "integer", nullable: true),
                    receiver_id = table.Column<int>(type: "integer", nullable: true),
                    content = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "message_status", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("messages_pkey", x => x.message_id);
                    table.ForeignKey(
                        name: "messages_receiver_id_fkey",
                        column: x => x.receiver_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "messages_sender_id_fkey",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    voucher_id = table.Column<int>(type: "integer", nullable: true),
                    order_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    tracking_number = table.Column<string>(type: "character varying(22)", maxLength: 22, nullable: true, defaultValueSql: "NULL::character varying"),
                    total_amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("orders_pkey", x => x.order_id);
                    table.ForeignKey(
                        name: "orders_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "orders_voucher_id_fkey",
                        column: x => x.voucher_id,
                        principalTable: "vouchers",
                        principalColumn: "voucher_id");
                });

            migrationBuilder.CreateTable(
                name: "threads",
                columns: table => new
                {
                    thread_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "text", nullable: true),
                    thumbnail_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    content = table.Column<string>(type: "text", nullable: true),
                    rating = table.Column<double>(type: "double precision", nullable: true, defaultValueSql: "0"),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("threads_pkey", x => x.thread_id);
                    table.ForeignKey(
                        name: "threads_created_by_fkey",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "tickets",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sender_id = table.Column<int>(type: "integer", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: true),
                    ticket_type = table.Column<string>(type: "text", nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    attachment_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tickets_pkey", x => x.id);
                    table.ForeignKey(
                        name: "tickets_sender_id_fkey",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    of_user = table.Column<int>(type: "integer", nullable: true),
                    reference_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    content = table.Column<string>(type: "text", nullable: true),
                    amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    type = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("transactions_pkey", x => x.id);
                    table.ForeignKey(
                        name: "transactions_of_user_fkey",
                        column: x => x.of_user,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "wallet",
                columns: table => new
                {
                    wallet_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    balance = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true, defaultValueSql: "0"),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("wallet_pkey", x => x.wallet_id);
                    table.ForeignKey(
                        name: "wallet_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "tables",
                columns: table => new
                {
                    table_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    room_id = table.Column<int>(type: "integer", nullable: true),
                    fee = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    gameExtension_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tables_pkey", x => x.table_id);
                    table.ForeignKey(
                        name: "tables_gameExtension_id_fkey",
                        column: x => x.gameExtension_id,
                        principalTable: "gameExtensions",
                        principalColumn: "extension_id");
                    table.ForeignKey(
                        name: "tables_room_id_fkey",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "room_id");
                });

            migrationBuilder.CreateTable(
                name: "courses_slot",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    room_id = table.Column<int>(type: "integer", nullable: true),
                    course_id = table.Column<int>(type: "integer", nullable: true),
                    instructor_id = table.Column<int>(type: "integer", nullable: true),
                    on_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    start_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    end_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("courses_slot_pkey", x => x.id);
                    table.ForeignKey(
                        name: "courses_slot_course_id_fkey",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "course_id");
                    table.ForeignKey(
                        name: "courses_slot_instructor_id_fkey",
                        column: x => x.instructor_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "courses_slot_room_id_fkey",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "room_id");
                });

            migrationBuilder.CreateTable(
                name: "prices",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    game_type_id = table.Column<int>(type: "integer", nullable: true),
                    product_id = table.Column<int>(type: "integer", nullable: true),
                    course_id = table.Column<int>(type: "integer", nullable: true),
                    member_fee = table.Column<BitArray>(type: "bit(1)", nullable: true),
                    teaching_salary = table.Column<BitArray>(type: "bit(1)", nullable: true),
                    price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("prices_pkey", x => x.id);
                    table.ForeignKey(
                        name: "prices_course_id_fkey",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "course_id");
                    table.ForeignKey(
                        name: "prices_game_type_id_fkey",
                        column: x => x.game_type_id,
                        principalTable: "gameTypes",
                        principalColumn: "type_id");
                    table.ForeignKey(
                        name: "prices_product_id_fkey",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id");
                });

            migrationBuilder.CreateTable(
                name: "users_courses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    course_id = table.Column<int>(type: "integer", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    result = table.Column<string>(type: "text", nullable: false),
                    enrolled_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    participant_status = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_courses_pkey", x => x.id);
                    table.ForeignKey(
                        name: "users_courses_course_id_fkey",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "course_id");
                    table.ForeignKey(
                        name: "users_courses_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "order_details",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<int>(type: "integer", nullable: true),
                    product_id = table.Column<int>(type: "integer", nullable: true),
                    quantity = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
                    subtotal = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("order_details_pkey", x => x.id);
                    table.ForeignKey(
                        name: "order_details_order_id_fkey",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id");
                    table.ForeignKey(
                        name: "order_details_product_id_fkey",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id");
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    order_id = table.Column<int>(type: "integer", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("payments_pkey", x => x.id);
                    table.ForeignKey(
                        name: "payments_order_id_fkey",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id");
                    table.ForeignKey(
                        name: "payments_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    comment_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    reply_to = table.Column<int>(type: "integer", nullable: true),
                    thread_id = table.Column<int>(type: "integer", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    content = table.Column<string>(type: "text", nullable: true),
                    rating = table.Column<double>(type: "double precision", nullable: true, defaultValueSql: "0"),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("comments_pkey", x => x.comment_id);
                    table.ForeignKey(
                        name: "comments_reply_to_fkey",
                        column: x => x.reply_to,
                        principalTable: "comments",
                        principalColumn: "comment_id");
                    table.ForeignKey(
                        name: "comments_thread_id_fkey",
                        column: x => x.thread_id,
                        principalTable: "threads",
                        principalColumn: "thread_id");
                    table.ForeignKey(
                        name: "comments_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "images",
                columns: table => new
                {
                    image_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    product_id = table.Column<int>(type: "integer", nullable: true),
                    thread_id = table.Column<int>(type: "integer", nullable: true),
                    url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("images_pkey", x => x.image_id);
                    table.ForeignKey(
                        name: "images_product_id_fkey",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id");
                    table.ForeignKey(
                        name: "images_thread_id_fkey",
                        column: x => x.thread_id,
                        principalTable: "threads",
                        principalColumn: "thread_id");
                });

            migrationBuilder.CreateTable(
                name: "threads_tags",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    thread_id = table.Column<int>(type: "integer", nullable: true),
                    tag_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("threads_tags_pkey", x => x.id);
                    table.ForeignKey(
                        name: "threads_tags_tag_id_fkey",
                        column: x => x.tag_id,
                        principalTable: "tags",
                        principalColumn: "tag_id");
                    table.ForeignKey(
                        name: "threads_tags_thread_id_fkey",
                        column: x => x.thread_id,
                        principalTable: "threads",
                        principalColumn: "thread_id");
                });

            migrationBuilder.CreateTable(
                name: "tables_appointments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    table_id = table.Column<int>(type: "integer", nullable: true),
                    appointment_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tables_appointments_pkey", x => x.id);
                    table.ForeignKey(
                        name: "tables_appointments_appointment_id_fkey",
                        column: x => x.appointment_id,
                        principalTable: "appointments",
                        principalColumn: "appointment_id");
                    table.ForeignKey(
                        name: "tables_appointments_table_id_fkey",
                        column: x => x.table_id,
                        principalTable: "tables",
                        principalColumn: "table_id");
                });

            migrationBuilder.CreateTable(
                name: "likes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    comment_id = table.Column<int>(type: "integer", nullable: true),
                    thread_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("likes_pkey", x => x.id);
                    table.ForeignKey(
                        name: "likes_comment_id_fkey",
                        column: x => x.comment_id,
                        principalTable: "comments",
                        principalColumn: "comment_id");
                    table.ForeignKey(
                        name: "likes_thread_id_fkey",
                        column: x => x.thread_id,
                        principalTable: "threads",
                        principalColumn: "thread_id");
                    table.ForeignKey(
                        name: "likes_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_appointments_user_id",
                table: "appointments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_cart_items_cart_id",
                table: "cart_items",
                column: "cart_id");

            migrationBuilder.CreateIndex(
                name: "IX_cart_items_product_id",
                table: "cart_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_comments_reply_to",
                table: "comments",
                column: "reply_to");

            migrationBuilder.CreateIndex(
                name: "IX_comments_thread_id",
                table: "comments",
                column: "thread_id");

            migrationBuilder.CreateIndex(
                name: "IX_comments_user_id",
                table: "comments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_courses_instructor_id",
                table: "courses",
                column: "instructor_id");

            migrationBuilder.CreateIndex(
                name: "IX_courses_slot_course_id",
                table: "courses_slot",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_courses_slot_instructor_id",
                table: "courses_slot",
                column: "instructor_id");

            migrationBuilder.CreateIndex(
                name: "IX_courses_slot_room_id",
                table: "courses_slot",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "IX_events_user_id",
                table: "events",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_friendlists_friend_id",
                table: "friendlists",
                column: "friend_id");

            migrationBuilder.CreateIndex(
                name: "IX_friendlists_user_id",
                table: "friendlists",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_friendrequests_from_user",
                table: "friendrequests",
                column: "from_user");

            migrationBuilder.CreateIndex(
                name: "IX_friendrequests_to_user",
                table: "friendrequests",
                column: "to_user");

            migrationBuilder.CreateIndex(
                name: "IX_gameExtensions_type_id",
                table: "gameExtensions",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "IX_images_product_id",
                table: "images",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_images_thread_id",
                table: "images",
                column: "thread_id");

            migrationBuilder.CreateIndex(
                name: "IX_likes_comment_id",
                table: "likes",
                column: "comment_id");

            migrationBuilder.CreateIndex(
                name: "IX_likes_thread_id",
                table: "likes",
                column: "thread_id");

            migrationBuilder.CreateIndex(
                name: "IX_likes_user_id",
                table: "likes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_messages_receiver_id",
                table: "messages",
                column: "receiver_id");

            migrationBuilder.CreateIndex(
                name: "IX_messages_sender_id",
                table: "messages",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_details_order_id",
                table: "order_details",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_details_product_id",
                table: "order_details",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_user_id",
                table: "orders",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_voucher_id",
                table: "orders",
                column: "voucher_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_order_id",
                table: "payments",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_user_id",
                table: "payments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_prices_course_id",
                table: "prices",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_prices_game_type_id",
                table: "prices",
                column: "game_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_prices_product_id",
                table: "prices",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_tags_product_id",
                table: "product_tags",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_tags_tag_id",
                table: "product_tags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_tables_gameExtension_id",
                table: "tables",
                column: "gameExtension_id");

            migrationBuilder.CreateIndex(
                name: "IX_tables_room_id",
                table: "tables",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "IX_tables_appointments_appointment_id",
                table: "tables_appointments",
                column: "appointment_id");

            migrationBuilder.CreateIndex(
                name: "IX_tables_appointments_table_id",
                table: "tables_appointments",
                column: "table_id");

            migrationBuilder.CreateIndex(
                name: "IX_threads_created_by",
                table: "threads",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_threads_tags_tag_id",
                table: "threads_tags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_threads_tags_thread_id",
                table: "threads_tags",
                column: "thread_id");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_sender_id",
                table: "tickets",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_of_user",
                table: "transactions",
                column: "of_user");

            migrationBuilder.CreateIndex(
                name: "IX_users_cart_id",
                table: "users",
                column: "cart_id");

            migrationBuilder.CreateIndex(
                name: "users_email_key",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_phone_key",
                table: "users",
                column: "phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_username_key",
                table: "users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_courses_course_id",
                table: "users_courses",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_courses_user_id",
                table: "users_courses",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_wallet_user_id",
                table: "wallet",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cart_items");

            migrationBuilder.DropTable(
                name: "courses_slot");

            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "friendlists");

            migrationBuilder.DropTable(
                name: "friendrequests");

            migrationBuilder.DropTable(
                name: "images");

            migrationBuilder.DropTable(
                name: "likes");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "order_details");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "prices");

            migrationBuilder.DropTable(
                name: "product_tags");

            migrationBuilder.DropTable(
                name: "tables_appointments");

            migrationBuilder.DropTable(
                name: "threads_tags");

            migrationBuilder.DropTable(
                name: "tickets");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "users_courses");

            migrationBuilder.DropTable(
                name: "wallet");

            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "appointments");

            migrationBuilder.DropTable(
                name: "tables");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "courses");

            migrationBuilder.DropTable(
                name: "threads");

            migrationBuilder.DropTable(
                name: "vouchers");

            migrationBuilder.DropTable(
                name: "gameExtensions");

            migrationBuilder.DropTable(
                name: "rooms");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "gameTypes");

            migrationBuilder.DropTable(
                name: "carts");

            migrationBuilder.Sql("DROP TYPE course_slot_status;");
            migrationBuilder.Sql("DROP TYPE course_status;");
            migrationBuilder.Sql("DROP TYPE event_status;");
            migrationBuilder.Sql("DROP TYPE event_type;");
            migrationBuilder.Sql("DROP TYPE game_extension;");
            migrationBuilder.Sql("DROP TYPE game_type;");
            migrationBuilder.Sql("DROP TYPE gender;");
            migrationBuilder.Sql("DROP TYPE message_status;");
            migrationBuilder.Sql("DROP TYPE order_status;");
            migrationBuilder.Sql("DROP TYPE participant_status;");
            migrationBuilder.Sql("DROP TYPE product_status;");
            migrationBuilder.Sql("DROP TYPE ranking;");
            migrationBuilder.Sql("DROP TYPE request_status;");
            migrationBuilder.Sql("DROP TYPE room_status;");
            migrationBuilder.Sql("DROP TYPE room_type;");
            migrationBuilder.Sql("DROP TYPE skill_level;");
            migrationBuilder.Sql("DROP TYPE thread_status;");
            migrationBuilder.Sql("DROP TYPE ticket_type;");
            migrationBuilder.Sql("DROP TYPE transaction_type;");
            migrationBuilder.Sql("DROP TYPE user_course_result;");
            migrationBuilder.Sql("DROP TYPE user_role;");
            migrationBuilder.Sql("DROP TYPE voucher_status;");
            migrationBuilder.Sql("DROP TYPE wallet_status;");
        }
    }
}
