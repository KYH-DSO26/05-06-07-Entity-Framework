using FailSafeRentals.Core.Models;
using FailSafeRentals.Data;

using Microsoft.EntityFrameworkCore;

using System.Diagnostics;

using var _db = new FailSafeRentalsDbContext();

// UPPGIFT 1: Create + automatgenererat Id
//SkapaNyKund();
//SkapaNyttFordon();

// UPPGIFT 2: Read + filtrering & sortering
//ListaBilar();

// UPPGIFT 3: .Find(1) vs .FirstOrDefault() (Minnescache vs SQL)
//ListaBil();

// UPPGIFT 4: Update (Change Tracker uppdaterar bara ändrad kolumn)
//UppdateraFordon();

// UPPGIFT 5: Create + Update i samma transaktion
//SkapaUthyrning();
//SkapaUthyrning2();

// UPPGIFT 6: Avslöja och åtgärda N+1-problemet
// ListaUthyrningsDetaljer();

// UPPGIFT 7: Slimmad rapport med .AsNoTracking() och .Select()
//SlimmadRapport();

// UPPGIFT 8: Läs in vehicles.txt
//LäsInTextFil();

// UPPGIFT 9: Realtidsväder från Open-Meteo API
await HämtaVäderDataAsync();

// UPPGIFT 10: Många-till-många med ExtraFeatures
LäggTillExtraFeatures();

// UPPGIFT 11: Säkerhetsverifiering mot SQL Injection
TestaSqlInjection();

// UPPGIFT 12: Säkert rå-SQL-anrop med .FromSqlInterpolated()
HämtaFordonSäkerRåSql();

// UPPGIFT 13: Anropa Stored Procedure (usp_) via EF Core
AnropaStoredProcedure();

// UPPGIFT 14: Aggregering och intäktsstatistik med .GroupBy()
VisaIntäktsStatistikPerMärke();

// UPPGIFT 15: Bygg och testa återanvändbar RentalService inför Lektion 7
await TestaRentalServiceAsync();


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
void SkapaNyttFordon()
{
    var vehicle = new Vehicle
    {
        RegistrationNumber = "TST001",
        Brand = "Chevrolet",
        Model = "Caprice",
        DailyRate = 975m,
        IsAvailable = true
    };
    _db.Vehicles.Add(vehicle);
    _db.SaveChanges();
    Console.WriteLine($"Nytt fordon skapat med Id: {vehicle.Id}");
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

void UppdateraFordon()
{
    var carByReg = _db.Vehicles
        .FirstOrDefault(v => v.RegistrationNumber == "ABC123");
    if ( carByReg is not null)
    {
        carByReg.DailyRate += 50m;
        _db.SaveChanges();          // Titta i loggen: UPDATE [Vehicles] SET [DailyRate] = @p0  WHERE [Id] = @p1;
    }
}

void SkapaUthyrning()
{
    var rentalVehicle = _db.Vehicles.First(v => v.IsAvailable);
    var customer = _db.Customers.Find(1);

    var rental = new Rental
    {
        CustomerId = customer.Id,
        VehicleId = rentalVehicle.Id,
        StartDate = DateTime.UtcNow,
        EndDate = DateTime.UtcNow.AddDays(3),
        TotalCost = rentalVehicle.DailyRate * 3
    };
    
    rentalVehicle.IsAvailable = false;
    _db.Rentals.Add(rental);

    _db.SaveChanges();  // Både INSERT och UPDATE körs i samma transaktion!
}
void SkapaUthyrning2()
{
    var rentalVehicle = _db.Vehicles.Find(2);
    var customer = _db.Customers.Find(2);

    var rental = new Rental
    {
        CustomerId = customer.Id,
        VehicleId = rentalVehicle.Id,
        StartDate = DateTime.UtcNow,
        EndDate = DateTime.UtcNow.AddDays(3),
        TotalCost = rentalVehicle.DailyRate * 3
    };

    rentalVehicle.IsAvailable = false;
    _db.Rentals.Add(rental);

    _db.SaveChanges();  // Både INSERT och UPDATE körs i samma transaktion!
}

void ListaUthyrningsDetaljer()
{
    // Här gör vi på rätt sätt. Det råkar vara det som känns mest naturligt...
    var rentalsWithDetails = _db.Rentals
        .Include(r => r.Customer)
        .Include(r => r.Vehicle)
        .ToList();
    Console.WriteLine("\n=== Hämta data på RÄTT sätt ===");
    foreach (var r in rentalsWithDetails)
    {
        Console.WriteLine($"{r.Customer.FullName} hyr {r.Vehicle.RegistrationNumber}");
    }

    //Console.WriteLine("\n=== Hämta data på FEL sätt ===");
    //var _dB2 = new FailSafeRentalsDbContext();      // Undvik eventuell cachning i _db

    //// Fråga 1: Hämtar alla uthyrningar (N rader)
    //var rentals = _dB2.Rentals.ToList();

    //foreach (var r in rentals)
    //{
    //    // +1 fråga per rad för att hämta ut Customer
    //    _dB2.Entry(r).Reference(x => x.Customer).Load();

    //    // +1 fråga för att hämta Vehicle
    //    _dB2.Entry(r).Reference(x => x.Vehicle).Load();

    //    Console.WriteLine($"{r.Customer.FullName} hyr {r.Vehicle.RegistrationNumber}");
    //}
}

void SlimmadRapport()
{
    var expensiveRentals = _db.Rentals
        .AsNoTracking()
        .Where(r => r.TotalCost > 1500m)
        .Select(r => new
        {
            RentalId = r.Id,
            CustomerName = r.Customer.FullName,
            VehicleInfo = r.Vehicle.Brand + " " + r.Vehicle.Model,
            TotalCost = r.TotalCost
        })
        .ToList();

    foreach (var item in expensiveRentals)
    {
        Console.WriteLine($"#{item.RentalId}: {item.CustomerName} -" 
            + " {item.VehicleInfo} ({item.TotalCost:C})");
    }

}

void LäsInTextFil()
{
    var importer = new VehicleImporter(_db);
    importer.ImportFromTableFile("vehicles.txt");
}

async Task HämtaVäderDataAsync()
{
    // UPPGIFT 9: Realtidsväder från Open-Meteo API
    var weatherService = new WeatherService();
    var weather = await weatherService.GetGothenburgWeatherAsync();

    if (weather is not null)
    {
        Console.WriteLine($"Väder i Göteborg: {weather.Temperature} °C, Vind: {weather.Windspeed} km/h");

        // Kombinera väderdata med LINQ-fråga mot _db.Vehicles
        var recommendedVehicles = weather.Temperature < 5.0
            ? _db.Vehicles.Where(v => v.IsAvailable && v.DailyRate >= 700m).ToList() // Rekommendera större/tryggare bilar vid kyla
            : _db.Vehicles.Where(v => v.IsAvailable).OrderBy(v => v.DailyRate).ToList();

        Console.WriteLine("Rekommenderade fordon utifrån dagens väder:");
        foreach (var v in recommendedVehicles.Take(3))
        {
            Console.WriteLine($"  - {v.Brand} {v.Model} ({v.RegistrationNumber}) - {v.DailyRate:C}/dag");
        }
    }

}

void LäggTillExtraFeatures()
{
    // UPPGIFT 10: Många-till-många med ExtraFeatures
    var vehicleWithFeatures = _db.Vehicles
        .Include(v => v.ExtraFeatures)
        .FirstOrDefault();

    if (vehicleWithFeatures is not null)
    {
        var winterTires = new ExtraFeature
        {
            Name = "Vinterdäck Dubb",
            DailyPrice = 150m
        };

        var gps = new ExtraFeature
        {
            Name = "GPS Navigation",
            DailyPrice = 75m
        };

        vehicleWithFeatures.ExtraFeatures.Add(winterTires);
        vehicleWithFeatures.ExtraFeatures.Add(gps);

        _db.SaveChanges();
        Console.WriteLine($"Lade till {vehicleWithFeatures.ExtraFeatures.Count} tillval på {vehicleWithFeatures.RegistrationNumber}.");
    }
}

void TestaSqlInjection()
{
    // UPPGIFT 11: Säkerhetsverifiering mot SQL Injection
    Console.Write("Ange registreringsnummer att söka efter (testa ' OR 1=1; DROP TABLE Vehicles; --): ");
    string? input = Console.ReadLine();

    var vehicles = _db.Vehicles
        .Where(v => v.RegistrationNumber == input)
        .ToList();

    // Förklaring (syns i .LogTo()):
    // EF Core översätter variabeln 'input' till en SQL-parameter (@__input_0) via sp_executesql.
    // SQL Server behandlar hela inmatningen som en ren textsträng att jämföra mot kolumnen,
    // aldrig som exekverbar SQL-kod. Tabellen Vehicles kan därför aldrig droppas.
    Console.WriteLine($"Antal träffar: {vehicles.Count}");
}

void HämtaFordonSäkerRåSql()
{
    // UPPGIFT 12: Säkra vs osäkra anrop med .FromSqlInterpolated()
    string brandInput = "Volvo";

    // SÄKERT: Trots $-sträng tar FromSqlInterpolated emot en FormattableString
    // och omvandlar {brandInput} till parametern @p0 i SQL Server.
    // (Varning: .FromSqlRaw($"... {brandInput}") hade däremot varit öppet för SQL Injection!)
    var brandVehicles = _db.Vehicles
    .FromSql($"SELECT * FROM dbo.Vehicles WHERE Brand = {brandInput}")
    .ToList();


    Console.WriteLine($"Hittade {brandVehicles.Count} bilar av märket {brandInput}.");
}

void AnropaStoredProcedure()
{
    // UPPGIFT 13: Anropa egen Stored Procedure (dbo.usp_GetVehiclesAboveRate)
    // Förutsätter att usp_GetVehiclesAboveRate är skapad i databasen.
    decimal minDailyRate = 700m;

    var premiumVehicles = _db.Vehicles
    .FromSql($"EXEC dbo.usp_GetVehiclesAboveRate @MinDailyRate = {minDailyRate}")
    .ToList();


    Console.WriteLine($"Fordon med dygnspris minst {minDailyRate:C}:");
    foreach (var v in premiumVehicles)
    {
        Console.WriteLine($"  - {v.Brand} {v.Model} ({v.RegistrationNumber}): {v.DailyRate:C}");
    }
}

void VisaIntäktsStatistikPerMärke()
{
    // UPPGIFT 14: Aggregering och intäktsstatistik med .GroupBy()
    // Hela GROUP BY-, COUNT-, SUM- och AVG-beräkningen körs nere i SQL Server.
    var brandStats = _db.Rentals
        .AsNoTracking()
        .GroupBy(r => r.Vehicle.Brand)
        .Select(g => new
        {
            Brand = g.Key,
            TotalRentals = g.Count(),
            TotalRevenue = g.Sum(r => r.TotalCost),
            AverageCost = g.Average(r => r.TotalCost)
        })
        .OrderByDescending(s => s.TotalRevenue)
        .ToList();

    Console.WriteLine("Intäktsstatistik per bilmärke:");
    foreach (var stat in brandStats)
    {
        Console.WriteLine($"  - {stat.Brand}: {stat.TotalRentals} st | Total: {stat.TotalRevenue:C} | Snitt: {stat.AverageCost:C}");
    }
}

async Task TestaRentalServiceAsync()
{
    // UPPGIFT 15: Testa återanvändbar RentalService inför Lektion 7
    var rentalService = new RentalService(_db);

    var available = await rentalService.GetAvailableVehiclesAsync();
    Console.WriteLine($"RentalService hittade {available.Count} tillgängliga bilar.");

    var customer = await _db.Customers.FirstOrDefaultAsync();
    var firstCar = available.FirstOrDefault();

    if (customer is not null && firstCar is not null)
    {
        var newRental = await rentalService.CreateRentalAsync(customer.Id, firstCar.Id, days: 2);
        Console.WriteLine($"Ny bokning skapad via RentalService! RentalId: {newRental.Id}, Kostnad: {newRental.TotalCost:C}");
    }
}
