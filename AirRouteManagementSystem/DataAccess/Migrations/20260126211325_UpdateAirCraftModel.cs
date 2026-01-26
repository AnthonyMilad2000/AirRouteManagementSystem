using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirRouteManagementSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAirCraftModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubImgId",
                table: "Aircrafts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubImgId",
                table: "Aircrafts",
                type: "int",
                nullable: true);
        }
    }
}
