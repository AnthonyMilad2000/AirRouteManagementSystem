using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirRouteManagementSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddMainImgAircraft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Aircrafts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Aircrafts");
        }
    }
}
