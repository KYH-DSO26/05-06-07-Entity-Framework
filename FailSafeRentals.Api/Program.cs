using FailSafeRentals.Core.Models;
using FailSafeRentals.Data;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Registrera tjänster i DI-containern
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Lägg till OpenAPI + Swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ProblemDetails ger enhetliga JSON-fel utan att läcka stack traces
builder.Services.AddProblemDetails();

// Koppla ihop DI-containern med vår anslutningssträng
var connString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionString saknas!");

builder.Services.AddDbContext<FailSafeRentalsDbContext>(options =>
    options.UseSqlServer(connString));

var app = builder.Build();
app.UseExceptionHandler();
app.UseStatusCodePages();




// 2. Konfigurera HTTP-pipelinen (Middleware)
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 3. Definiera endpoints & starta servern
app.MapGet("/api/status", () =>
{
    return Results.Ok(new
    {
        Service = "FailSafe Rentals API",
        Version = "1.0 (.NET 11)",
        Status = "Online"
    });
});

//app.MapGet("/api/vehicles", async (FailSafeRentalsDbContext _db) =>
//    Results.Ok(await _db.Vehicles.AsNoTracking().ToListAsync()));

//app.MapPost("/api/vehicles", async (Vehicle item, FailSafeRentalsDbContext _db) =>
//{
//    _db.Vehicles.Add(item);
//    await _db.SaveChangesAsync();
//    return Results.Created($"/api/vehicle/{item.Id}", item);
//});
// POST som tar emot CreateVehicleDto (utan Id & Rentals i Swagger!)
app.MapPost("/api/vehicles", async (CreateVehicleDto dto, FailSafeRentalsDbContext _db) =>
{
    // 1. Mappa manuellt från DTO till databas-entitet
    var vehicle = new Vehicle
    {
        RegistrationNumber = dto.RegistrationNumber,
        Brand = dto.Brand,
        Model = dto.Model,
        DailyRate = dto.DailyRate
    };

    _db.Vehicles.Add(vehicle);
    await _db.SaveChangesAsync();

    // 2. Returnera en ren VehicleDto tillbaka till klienten
    var resultDto = new VehicleDto(
        vehicle.Id,
        vehicle.RegistrationNumber,
        vehicle.Brand,
        vehicle.Model,
        vehicle.DailyRate
    );

    return Results.Created($"/api/vehicles/{vehicle.Id}", resultDto);
});



app.MapPut("/api/vehicles/{id:int}", async (int id, Vehicle input, FailSafeRentalsDbContext _db) =>
{
    var existing = await _db.Vehicles.FindAsync(id);
    if (existing is null) return Results.NotFound();
    existing.Brand = input.Brand;
    existing.Model = input.Model;
    existing.DailyRate = input.DailyRate;
    await _db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/api/vehicles/{id:int}", async (int id, FailSafeRentalsDbContext _db) =>
{
    var rows = await _db.Vehicles.Where(e => e.Id == id).ExecuteDeleteAsync();
    return rows == 0 ? Results.NotFound() : Results.NoContent();
});

// GET som projicerar direkt till DTO i SQL-frågan
app.MapGet("/api/vehicles", async (FailSafeRentalsDbContext _db) =>
    Results.Ok(await _db.Vehicles
        .Select(v => new VehicleDto(v.Id, v.RegistrationNumber, v.Brand, v.Model, v.DailyRate))
        .ToListAsync()));

app.MapGet("/api/vehicles/{id:int}", async (int id, FailSafeRentalsDbContext _db) =>
    await _db.Vehicles.FindAsync(id) is Vehicle item
        ? Results.Ok(item)
        : Results.Problem("Fordonet hittades inte", statusCode: 404));



// Starta servern
app.Run();

// Eventuella records och annat måste ligga här i botten
internal record VehicleDto(int Id, string RegistrationNumber, string Brand, string Model, decimal DailyRate);
internal record CreateVehicleDto(string RegistrationNumber, string Brand, string Model, decimal DailyRate);


