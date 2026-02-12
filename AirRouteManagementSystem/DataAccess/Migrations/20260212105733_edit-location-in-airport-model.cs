using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirRouteManagementSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class editlocationinairportmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Airports",
                newName: "LocationLink");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LocationLink",
                table: "Airports",
                newName: "Location");
        }
    }
}
