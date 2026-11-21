using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FailSafeRentals.Data.Migrations;

/// <inheritdoc />
public partial class _20261118151351_RemoveExtraLesson7Tables : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "DamageReports");

        migrationBuilder.DropTable(
            name: "FleetVehicleMaintenanceTag");

        migrationBuilder.DropTable(
            name: "FleetVehicles");

        migrationBuilder.DropTable(
            name: "MaintenanceTags");

        migrationBuilder.DropTable(
            name: "RentalStations");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "MaintenanceTags",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TagName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MaintenanceTags", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "RentalStations",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                City = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                StationCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                StreetAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RentalStations", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "FleetVehicles",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                RentalStationId = table.Column<int>(type: "int", nullable: false),
                DamageReportId = table.Column<int>(type: "int", nullable: false),
                Make = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                MileageKm = table.Column<int>(type: "int", nullable: false),
                Model = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                NeedsService = table.Column<bool>(type: "bit", nullable: false),
                RegistrationNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FleetVehicles", x => x.Id);
                table.ForeignKey(
                    name: "FK_FleetVehicles_RentalStations_RentalStationId",
                    column: x => x.RentalStationId,
                    principalTable: "RentalStations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "DamageReports",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FleetVehicleId = table.Column<int>(type: "int", nullable: false),
                AuditLoggedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                EstimatedCost = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                RepairedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DamageReports", x => x.Id);
                table.ForeignKey(
                    name: "FK_DamageReports_FleetVehicles_FleetVehicleId",
                    column: x => x.FleetVehicleId,
                    principalTable: "FleetVehicles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "FleetVehicleMaintenanceTag",
            columns: table => new
            {
                MaintenanceTagsId = table.Column<int>(type: "int", nullable: false),
                VehiclesId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FleetVehicleMaintenanceTag", x => new { x.MaintenanceTagsId, x.VehiclesId });
                table.ForeignKey(
                    name: "FK_FleetVehicleMaintenanceTag_FleetVehicles_VehiclesId",
                    column: x => x.VehiclesId,
                    principalTable: "FleetVehicles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_FleetVehicleMaintenanceTag_MaintenanceTags_MaintenanceTagsId",
                    column: x => x.MaintenanceTagsId,
                    principalTable: "MaintenanceTags",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData(
            table: "RentalStations",
            columns: new[] { "Id", "City", "StationCode", "StreetAddress" },
            values: new object[,]
            {
                { 1, "Göteborg", "GOT-01", "Landvetter flygplats 1" },
                { 2, "Stockholm", "STO-01", "Arlanda Terminal 5" },
                { 3, "Malmö", "MMX-01", "Sturupsvägen 3" }
            });

        migrationBuilder.CreateIndex(
            name: "IX_DamageReports_FleetVehicleId",
            table: "DamageReports",
            column: "FleetVehicleId");

        migrationBuilder.CreateIndex(
            name: "IX_FleetVehicleMaintenanceTag_VehiclesId",
            table: "FleetVehicleMaintenanceTag",
            column: "VehiclesId");

        migrationBuilder.CreateIndex(
            name: "IX_FleetVehicles_RegistrationNumber",
            table: "FleetVehicles",
            column: "RegistrationNumber",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_FleetVehicles_RentalStationId",
            table: "FleetVehicles",
            column: "RentalStationId");

        migrationBuilder.CreateIndex(
            name: "IX_RentalStations_StationCode",
            table: "RentalStations",
            column: "StationCode",
            unique: true);
    }
}
