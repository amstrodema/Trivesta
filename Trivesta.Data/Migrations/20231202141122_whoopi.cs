using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trivesta.Data.Migrations
{
    /// <inheritdoc />
    public partial class whoopi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFake",
                table: "Rooms",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFake",
                table: "Rooms");
        }
    }
}
