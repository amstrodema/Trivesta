using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trivesta.Data.Migrations
{
    /// <inheritdoc />
    public partial class updater2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Religion",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsApprovalRequired",
                table: "RoomTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "RoomMembers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "RoomMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "GuestCoin",
                table: "LoginMonitors",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Religion",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsApprovalRequired",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "RoomMembers");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "RoomMembers");

            migrationBuilder.DropColumn(
                name: "GuestCoin",
                table: "LoginMonitors");
        }
    }
}
