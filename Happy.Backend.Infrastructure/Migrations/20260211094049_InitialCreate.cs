using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Happy.Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "app_credentials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppSecret = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    AppName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_credentials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "seasons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SeasonName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FarmingType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TargetYield = table.Column<double>(type: "double precision", nullable: false),
                    YieldUnit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ExpectedProfit = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ScaleValue = table.Column<double>(type: "double precision", nullable: false),
                    ScaleUnit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EndDate = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "stock_ins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ProductCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProductName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Platform = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Image = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    StockInDate = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    OfflineTempCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_ins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sync_raw_farmer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Phone = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SyncTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SyncData = table.Column<string>(type: "jsonb", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    TotalItems = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ProcessedItems = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ErrorDetails = table.Column<string>(type: "jsonb", nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sync_raw_farmer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sync_raw_store",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Phone = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SyncTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SyncData = table.Column<string>(type: "jsonb", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    TotalItems = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ProcessedItems = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ErrorDetails = table.Column<string>(type: "jsonb", nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sync_raw_store", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "farmer_information",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppCredentialId = table.Column<int>(type: "integer", nullable: false),
                    UserName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FarmName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_farmer_information", x => x.Id);
                    table.ForeignKey(
                        name: "FK_farmer_information_app_credentials_AppCredentialId",
                        column: x => x.AppCredentialId,
                        principalTable: "app_credentials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "store_information",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppCredentialId = table.Column<int>(type: "integer", nullable: false),
                    StoreName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UserName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_information", x => x.Id);
                    table.ForeignKey(
                        name: "FK_store_information_app_credentials_AppCredentialId",
                        column: x => x.AppCredentialId,
                        principalTable: "app_credentials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "activities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeasonName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ActivityName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DateTime = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StatusLabel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ImageUrls = table.Column<string>(type: "jsonb", nullable: false),
                    ImageCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_activities_seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "expenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeasonName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ExpenName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Date = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsDebt = table.Column<bool>(type: "boolean", nullable: false),
                    ExpenPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ImageUrls = table.Column<string>(type: "jsonb", nullable: false),
                    ImageCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_expenses_seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "harvests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false),
                    HarvestName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    HarvestDate = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Quantity = table.Column<double>(type: "double precision", nullable: false),
                    QuantityUnit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Product = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Buyer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RecordDebt = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_harvests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_harvests_seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_activities_Phone",
                table: "activities",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_activities_SeasonId",
                table: "activities",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_app_credentials_AppSecret",
                table: "app_credentials",
                column: "AppSecret",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_expenses_Phone",
                table: "expenses",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_expenses_SeasonId",
                table: "expenses",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_farmer_information_AppCredentialId",
                table: "farmer_information",
                column: "AppCredentialId");

            migrationBuilder.CreateIndex(
                name: "IX_harvests_Phone",
                table: "harvests",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_harvests_SeasonId",
                table: "harvests",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_seasons_Phone",
                table: "seasons",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_stock_ins_ProductCode_Phone",
                table: "stock_ins",
                columns: new[] { "ProductCode", "Phone" });

            migrationBuilder.CreateIndex(
                name: "IX_store_information_AppCredentialId",
                table: "store_information",
                column: "AppCredentialId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activities");

            migrationBuilder.DropTable(
                name: "expenses");

            migrationBuilder.DropTable(
                name: "farmer_information");

            migrationBuilder.DropTable(
                name: "harvests");

            migrationBuilder.DropTable(
                name: "stock_ins");

            migrationBuilder.DropTable(
                name: "store_information");

            migrationBuilder.DropTable(
                name: "sync_raw_farmer");

            migrationBuilder.DropTable(
                name: "sync_raw_store");

            migrationBuilder.DropTable(
                name: "seasons");

            migrationBuilder.DropTable(
                name: "app_credentials");
        }
    }
}
