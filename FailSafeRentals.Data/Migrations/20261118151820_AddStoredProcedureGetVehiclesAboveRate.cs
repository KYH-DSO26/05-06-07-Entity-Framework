using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FailSafeRentals.Data.Migrations;

/// <inheritdoc />
public partial class _20261118151820_AddStoredProcedureGetVehiclesAboveRate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
        CREATE OR ALTER PROCEDURE dbo.usp_GetVehiclesAboveRate
            @MinDailyRate DECIMAL(10, 2)
        AS
        BEGIN
            SET NOCOUNT ON;
            SELECT Id, RegistrationNumber, Brand, Model, DailyRate, IsAvailable
            FROM dbo.Vehicles
            WHERE DailyRate >= @MinDailyRate
            ORDER BY DailyRate DESC;
        END
    ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.usp_GetVehiclesAboveRate;");
    }

}
