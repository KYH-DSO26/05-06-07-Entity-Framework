using FailSafeRentals.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace FailSafeRentals.Data;

public class RentalService(FailSafeRentalsDbContext _db)
{
    public async Task<List<Vehicle>> GetAvailableVehiclesAsync()
    {
        return await _db.Vehicles
            .AsNoTracking()
            .Where(v => v.IsAvailable)
            .OrderBy(v => v.DailyRate)
            .ToListAsync();
    }

    public async Task<Rental> CreateRentalAsync(int customerId, int vehicleId, int days)
    {
        var vehicle = await _db.Vehicles.FindAsync(vehicleId)
            ?? throw new InvalidOperationException($"Fordon med Id {vehicleId} hittades inte.");

        if (!vehicle.IsAvailable)
        {
            throw new InvalidOperationException($"Fordon {vehicle.RegistrationNumber} är redan uthyrt.");
        }

        var rental = new Rental
        {
            CustomerId = customerId,
            VehicleId = vehicleId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(days),
            TotalCost = vehicle.DailyRate * days
        };

        vehicle.IsAvailable = false;

        _db.Rentals.Add(rental);
        await _db.SaveChangesAsync();

        return rental;
    }
}
