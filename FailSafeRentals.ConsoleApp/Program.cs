using FailSafeRentals.Core.Models;
using FailSafeRentals.Data;

using Microsoft.EntityFrameworkCore;

using System.Diagnostics;

using var _db = new FailSafeRentalsDbContext();

// Uppgift 1
//SkapaNyKund();
ListaBilar();




if (Debugger.IsAttached)
{
    Console.Write("\n\nTryck på en tangent för att stänga fönstret...");
    Console.ReadKey();
}

// Skapa metoder för det mesta som vi gör i dagens lektion:
void SkapaNyKund()
{
    // Uppgift 1
    var customer = new Customer
    {
        FullName = "Maja Lindberg",
        Email = "maja.lindberg@failsafe.se",
        DriverLicenseNumber = "SE-1234567890"
    };
    _db.Customers.Add(customer);
    _db.SaveChanges();              // För att ändringen ska ta skruv, dvs landa i databsen
    Console.WriteLine($"Ny kund sparad med Id: {customer.Id}");
}

void ListaBilar()
{
    var availableVehicles = _db.Vehicles
        .Where(v => v.IsAvailable)
        .OrderBy(v => v.DailyRate)
        .ToList();

    foreach ( var v in availableVehicles)
    {
        Console.WriteLine($"{v.RegistrationNumber} | {v.Brand} {v.Model} | {v.DailyRate:C}/dag");
    }
}