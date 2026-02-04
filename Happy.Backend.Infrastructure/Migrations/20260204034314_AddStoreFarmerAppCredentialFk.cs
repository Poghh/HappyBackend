using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Happy.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreFarmerAppCredentialFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppCredentialId",
                table: "store_information",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AppCredentialId",
                table: "farmer_information",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_store_information_AppCredentialId",
                table: "store_information",
                column: "AppCredentialId");

            migrationBuilder.CreateIndex(
                name: "IX_farmer_information_AppCredentialId",
                table: "farmer_information",
                column: "AppCredentialId");

            migrationBuilder.AddForeignKey(
                name: "FK_farmer_information_app_credentials_AppCredentialId",
                table: "farmer_information",
                column: "AppCredentialId",
                principalTable: "app_credentials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_store_information_app_credentials_AppCredentialId",
                table: "store_information",
                column: "AppCredentialId",
                principalTable: "app_credentials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_farmer_information_app_credentials_AppCredentialId",
                table: "farmer_information");

            migrationBuilder.DropForeignKey(
                name: "FK_store_information_app_credentials_AppCredentialId",
                table: "store_information");

            migrationBuilder.DropIndex(
                name: "IX_store_information_AppCredentialId",
                table: "store_information");

            migrationBuilder.DropIndex(
                name: "IX_farmer_information_AppCredentialId",
                table: "farmer_information");

            migrationBuilder.DropColumn(
                name: "AppCredentialId",
                table: "store_information");

            migrationBuilder.DropColumn(
                name: "AppCredentialId",
                table: "farmer_information");
        }
    }
}
