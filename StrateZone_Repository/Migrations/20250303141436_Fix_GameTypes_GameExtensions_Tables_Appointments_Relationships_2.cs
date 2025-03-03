using Microsoft.EntityFrameworkCore.Migrations;
using StrateZone_Repository.Parameters;

#nullable disable

namespace StrateZone_Repository.Migrations
{
    /// <inheritdoc />
    public partial class Fix_GameTypes_GameExtensions_Tables_Appointments_Relationships_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "wallet",
                type: "text",
                nullable: false,
                oldClrType: typeof(PostgreEnums.WalletStatus),
                oldType: "wallet_status");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "rooms",
                type: "text",
                nullable: false,
                oldClrType: typeof(PostgreEnums.RoomStatus),
                oldType: "room_status");

            migrationBuilder.AlterColumn<string>(
                name: "room_type",
                table: "rooms",
                type: "text",
                nullable: false,
                oldClrType: typeof(PostgreEnums.RoomType),
                oldType: "room_type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<PostgreEnums.WalletStatus>(
                name: "status",
                table: "wallet",
                type: "wallet_status",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<PostgreEnums.RoomStatus>(
                name: "status",
                table: "rooms",
                type: "room_status",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<PostgreEnums.RoomType>(
                name: "room_type",
                table: "rooms",
                type: "room_type",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
