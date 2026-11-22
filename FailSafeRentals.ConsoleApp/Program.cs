using FailSafeRentals.Core.Models;
using FailSafeRentals.Data;

using Microsoft.EntityFrameworkCore;

using System.Diagnostics;

using var _db = new FailSafeRentalsDbContext();

// Read + filtrering & sortering
ListaBilar();

// .Find(1) vs .FirstOrDefault() (Minnescache vs SQL)
//ListaBil();



if (Debugger.IsAttached)
{
    Console.Write("\n\nTryck på en tangent för att stänga fönstret...");
    Console.ReadKey();
}

// Skapa metoder för det mesta som vi gör i dagens lektion:
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

void ListaBil()
{
    var carById = _db.Vehicles
        .Find(1);   // Kolla loggen: Ingen SQL om Id = 1 redan laddades i uppgift 2

    Console.WriteLine($"\nFind(1): {carById?.Model}\n");

    var carByReg = _db.Vehicles
        .FirstOrDefault(v => v.RegistrationNumber == "ABC123"); // Skickar alltid SELECT TOP (1)

    Console.WriteLine($"\nFirstOrDefault: {carById.RegistrationNumber} - {carById.Brand} {carById?.Model}\n");
}
