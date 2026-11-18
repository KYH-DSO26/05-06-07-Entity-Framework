using FailSafeRentals.Core.Models;
using FailSafeRentals.Data;

using Microsoft.EntityFrameworkCore;

using System.Diagnostics;

using var _db = new FailSafeRentalsDbContext();

// Uppgift 1
//SkapaNyKund();

// Uppgift 2
ListaBilar();

//Uppgift 3
ListaBil();

// Uppgift 4





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

void ListaBil()
{
    var carById = _db.Vehicles
        .Find(1);   // Kolla loggen: Ingen SQL om Id = 1 redan laddades i uppgift 2

    Console.WriteLine($"\nFind(1): {carById?.Model}\n");

    var carByReg = _db.Vehicles
        .FirstOrDefault(v => v.RegistrationNumber == "ABC123"); // Skickar alltid SELECT TOP (1)

    Console.WriteLine($"\nFirstOrDefault: {carById?.Model}\n");
}