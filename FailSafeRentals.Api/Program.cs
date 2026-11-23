using FailSafeRentals.Core.Models;
using FailSafeRentals.Data;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Dependency Injection för DbContext
builder.Services.AddDbContext<FailSafeRentalsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.MapGet("/api/status", (IWebHostEnvironment env) => Results.Ok(new
{
    Service = "FailSafe Rentals API",
    Version = "1.0.0",
    Environment = env.EnvironmentName
}));

app.MapGet("/api/features", async (FailSafeRentalsDbContext _db) =>
    Results.Ok(await _db.ExtraFeatures.AsNoTracking().ToListAsync()));

app.MapPost("/api/features", async (ExtraFeature item, FailSafeRentalsDbContext _db) =>
{
    _db.ExtraFeatures.Add(item);
    await _db.SaveChangesAsync();
    return Results.Created($"/api/features/{item.Id}", item);
});

// GET alla fordon
//app.MapGet("/api/vehicles", async (FailSafeRentalsDbContext _db) =>
//    Results.Ok(await _db.Vehicles.AsNoTracking().ToListAsync()));
// GET alla fordon med DTO
app.MapGet("api/vehicles", async (FailSafeRentalsDbContext _db) =>
{
    var vehicles = await _db.Vehicles
        .AsNoTracking()
        .Select(v => new VehicleDto(
            v.Id,
            v.RegistrationNumber,
            v.Brand,
            v.Model,
            v.DailyRate,
            v.IsAvailable))
        .ToListAsync();

    return Results.Ok(vehicles);
});

// GET enskilt fordon 
app.MapGet("/api/vehicles/{id:int}", async (int id,
FailSafeRentalsDbContext db) =>
{
    var vehicle = await db.Vehicles
    .AsNoTracking()
    .Where(v => v.Id == id)
    .Select(v => new VehicleDto(
        v.Id,
        v.RegistrationNumber,
        v.Brand,
        v.Model,
        v.DailyRate,
        v.IsAvailable))
    .FirstOrDefaultAsync();

    return vehicle is null
    ? Results.Problem($"Fordon med ID {id} hittades inte.", statusCode: 404)
    : Results.Ok(vehicle);
});


//app.MapPost("/api/vehicles", async (Vehicle item, FailSafeRentalsDbContext _db) =>
//{
//    _db.Vehicles.Add(item);
//    await _db.SaveChangesAsync();
//    return Results.Created($"/api/vehicles/{item.Id}", item);
//});

app.MapPost("api/vehicles", async (CreateVehicleDto dto, FailSafeRentalsDbContext _db) =>
{
    var vehicle = new Vehicle
    {
        RegistrationNumber = dto.RegistrationNumber,
        Brand = dto.Brand,
        Model = dto.Model,
        DailyRate = dto.DailyRate,
        IsAvailable = true
    };

    _db.Vehicles.Add(vehicle);
    await _db.SaveChangesAsync();

    var resultDto = new VehicleDto(
        vehicle.Id,
        vehicle.RegistrationNumber,
        vehicle.Brand,
        vehicle.Model,
        vehicle.DailyRate,
        vehicle.IsAvailable
        );

    return Results.Created($"/api/vehicles/{vehicle.Id}", resultDto);
});

// DELETE fordon med ProblemDetails 404
app.MapDelete("/api/vehicles", async (int id,
FailSafeRentalsDbContext _db) =>
{
    var vehicle = await _db.Vehicles.FindAsync(id);
    if (vehicle is null)
    {
        return Results.Problem($"Fordon med ID {id} hittades inte.", statusCode: 404);
    }

    _db.Vehicles.Remove(vehicle);
    await _db.SaveChangesAsync();
    return Results.NoContent();
});





app.Run();

// DTO- och andra records MÅSTE ligga sist (egentligen efter app.run)
public record VehicleDto(
    int Id,
    string RegistrationNumber,
    string Brand,
    string Model,
    decimal DailyRate,
    bool IsAvailable
    );
public record CreateVehicleDto(
    string RegistrationNumber,
    string Brand,
    string Model,
    decimal DailyRate
    );
