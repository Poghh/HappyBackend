using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Happy.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnFarmNameInFarmerInformation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FarmName",
                table: "farmer_information",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FarmName",
                table: "farmer_information");
        }
    }
}
