using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirRouteManagementSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTypeNameAircraft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Aircrafts",
                newName: "AircraftType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AircraftType",
                table: "Aircrafts",
                newName: "Type");
        }
    }
}
