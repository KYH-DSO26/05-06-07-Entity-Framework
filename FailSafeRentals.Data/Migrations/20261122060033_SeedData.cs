using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FailSafeRentals.Data.Migrations;

/// <inheritdoc />
public partial class _20261122060033_SeedData : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "Customers",
            columns: new[] { "Id", "DriverLicenseNumber", "Email", "FullName" },
            values: new object[,]
            {
                { 1, "19850412-1234", "anna.lindqvist@example.se", "Anna Lindqvist" },
                { 2, "19901123-5678", "erik.svensson@example.se", "Erik Svensson" },
                { 3, "19780205-9012", "maria.berg@example.se", "Maria Berg" },
                { 4, "19950719-3456", "johan.ekstrom@example.se", "Johan Ekström" },
                { 5, "19880930-7890", "sofia.holm@example.se", "Sofia Holm" }
            });

        migrationBuilder.InsertData(
            table: "ExtraFeatures",
            columns: new[] { "Id", "DailyPrice", "Name" },
            values: new object[,]
            {
                { 1, 75m, "GPS-navigator" },
                { 2, 100m, "Barnstol" },
                { 3, 150m, "Takbox" },
                { 4, 50m, "Vinterdäck" },
                { 5, 200m, "Extra försäkring" }
            });

        migrationBuilder.InsertData(
            table: "Vehicles",
            columns: new[] { "Id", "Brand", "DailyRate", "IsAvailable", "Model", "RegistrationNumber" },
            values: new object[,]
            {
                { 1, "Volvo", 850m, true, "C30", "ABC234" },
                { 2, "Polestar", 1200m, false, "2", "DEF567" },
                { 3, "Volkswagen", 700m, true, "Golf", "GHI890" },
                { 4, "BMW", 1100m, true, "320i", "JKL123" },
                { 5, "Toyota", 550m, false, "Yaris", "MNO456" }
            });

        migrationBuilder.InsertData(
            table: "ExtraFeatureVehicle",
            columns: new[] { "ExtraFeaturesId", "VehiclesId" },
            values: new object[,]
            {
                { 1, 1 },
                { 2, 3 },
                { 3, 4 },
                { 4, 1 },
                { 5, 2 }
            });

        migrationBuilder.InsertData(
            table: "Rentals",
            columns: new[] { "Id", "CustomerId", "EndDate", "StartDate", "TotalCost", "VehicleId" },
            values: new object[,]
            {
                { 1, 1, new DateTime(2026, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 2550m, 1 },
                { 2, 2, null, new DateTime(2026, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, 2 },
                { 3, 3, new DateTime(2026, 2, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1400m, 3 },
                { 4, 4, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 5500m, 4 },
                { 5, 5, null, new DateTime(2026, 11, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, 5 }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "ExtraFeatureVehicle",
            keyColumns: new[] { "ExtraFeaturesId", "VehiclesId" },
            keyValues: new object[] { 1, 1 });

        migrationBuilder.DeleteData(
            table: "ExtraFeatureVehicle",
            keyColumns: new[] { "ExtraFeaturesId", "VehiclesId" },
            keyValues: new object[] { 2, 3 });

        migrationBuilder.DeleteData(
            table: "ExtraFeatureVehicle",
            keyColumns: new[] { "ExtraFeaturesId", "VehiclesId" },
            keyValues: new object[] { 3, 4 });

        migrationBuilder.DeleteData(
            table: "ExtraFeatureVehicle",
            keyColumns: new[] { "ExtraFeaturesId", "VehiclesId" },
            keyValues: new object[] { 4, 1 });

        migrationBuilder.DeleteData(
            table: "ExtraFeatureVehicle",
            keyColumns: new[] { "ExtraFeaturesId", "VehiclesId" },
            keyValues: new object[] { 5, 2 });

        migrationBuilder.DeleteData(
            table: "Rentals",
            keyColumn: "Id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "Rentals",
            keyColumn: "Id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "Rentals",
            keyColumn: "Id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "Rentals",
            keyColumn: "Id",
            keyValue: 4);

        migrationBuilder.DeleteData(
            table: "Rentals",
            keyColumn: "Id",
            keyValue: 5);

        migrationBuilder.DeleteData(
            table: "Customers",
            keyColumn: "Id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "Customers",
            keyColumn: "Id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "Customers",
            keyColumn: "Id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "Customers",
            keyColumn: "Id",
            keyValue: 4);

        migrationBuilder.DeleteData(
            table: "Customers",
            keyColumn: "Id",
            keyValue: 5);

        migrationBuilder.DeleteData(
            table: "ExtraFeatures",
            keyColumn: "Id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "ExtraFeatures",
            keyColumn: "Id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "ExtraFeatures",
            keyColumn: "Id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "ExtraFeatures",
            keyColumn: "Id",
            keyValue: 4);

        migrationBuilder.DeleteData(
            table: "ExtraFeatures",
            keyColumn: "Id",
            keyValue: 5);

        migrationBuilder.DeleteData(
            table: "Vehicles",
            keyColumn: "Id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "Vehicles",
            keyColumn: "Id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "Vehicles",
            keyColumn: "Id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "Vehicles",
            keyColumn: "Id",
            keyValue: 4);

        migrationBuilder.DeleteData(
            table: "Vehicles",
            keyColumn: "Id",
            keyValue: 5);
    }
}
