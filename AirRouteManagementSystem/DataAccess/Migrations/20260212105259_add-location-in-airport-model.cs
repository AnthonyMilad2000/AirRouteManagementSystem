using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirRouteManagementSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addlocationinairportmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Airports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Airports");
        }
    }
}
