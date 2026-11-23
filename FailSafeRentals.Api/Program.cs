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


// Alla ändpunkter kommer här, före app.Run()
app.MapGet("/api/status", (IWebHostEnvironment env) => Results.Ok(new
{
    Service = "FailSafe Rentals API",
    Version = "1.2.0",
    Environment = env.EnvironmentName
}));

#region /api/features
app.MapGet("/api/features", async (FailSafeRentalsDbContext _db) =>
{
    var features = await _db.ExtraFeatures
        .AsNoTracking()
        .Select(f => new ExtraFeatureDto(
            f.Id,
            f.Name,
            f.DailyPrice))
        .ToListAsync();

    return Results.Ok(features);
});

// GET enskild extrautrustning
app.MapGet("/api/features/{id:int}", async (int id,
FailSafeRentalsDbContext _db) =>
{
    var feature = await _db.ExtraFeatures
    .AsNoTracking()
    .Where(f => f.Id == id)
    .Select(f => new ExtraFeatureDto(
        f.Id,
        f.Name,
        f.DailyPrice))
    .FirstOrDefaultAsync();

    return feature is null
    ? Results.Problem($"Extratrustning med ID {id} hittades inte.", statusCode: 404)
    : Results.Ok(feature);
});

app.MapPost("/api/features", async (CreateExtraFeatureDto item, FailSafeRentalsDbContext _db) =>
{
    var feature = new ExtraFeature
    {
        Name = item.Name,
        DailyPrice = item.DailyPrice
    };
    _db.ExtraFeatures.Add(feature);
    await _db.SaveChangesAsync();

    var resultDto = new ExtraFeatureDto(
        feature.Id,
        feature.Name,
        feature.DailyPrice
    );
    
    return Results.Created($"/api/features/{resultDto.Id}", resultDto);
});
#endregion

#region /api/vehicles
// GET fordon med dynamisk IQueryable-filtrering & AsNoTracking (Uppgift 6 & 13)
app.MapGet("/api/vehicles", async (
    string? brand,
    decimal? maxDailyRate,
    bool? onlyAvailable,
    FailSafeRentalsDbContext db) =>
    {
        var query = db.Vehicles.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(brand))
            query = query.Where(v => v.Brand.Contains(brand));
        if (maxDailyRate.HasValue)
            query = query.Where(v => v.DailyRate <= maxDailyRate.Value);
        if (onlyAvailable == true)
            query = query.Where(v => v.IsAvailable);
        var vehicles = await query
        .Select(v => new VehicleDto(
        v.Id,
        v.RegistrationNumber,
        v.Brand,
        v.Model,
        v.DailyRate,
        v.IsAvailable))
        .ToListAsync();
        return Results.Ok(vehicles);
    }
);

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

//SKAPA fordon
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
#endregion

#region /api/customers
// GET alla kunder med DTO (Uppgift 7)
app.MapGet("/api/customers", async (FailSafeRentalsDbContext _db) =>
{
    var customers = await _db.Customers
    .AsNoTracking()
    .Select(c => new CustomerDto(
        c.Id,
        c.FullName,
        c.Email,
        c.DriverLicenseNumber))
    .ToListAsync();
    return Results.Ok(customers);
});
// GET enskild kund med ProblemDetails 404 (Uppgift 7 & 8)
app.MapGet("/api/customers/{id:int}", async (int id,
FailSafeRentalsDbContext _db) =>
{
    var customer = await _db.Customers
    .AsNoTracking()
    .Where(c => c.Id == id)
    .Select(c => new CustomerDto(
    c.Id,
    c.FullName,
    c.Email,
    c.DriverLicenseNumber))
    .FirstOrDefaultAsync();
    return customer is null
    ? Results.Problem($"Kund med ID {id} hittades inte.",
    statusCode: 404)
    : Results.Ok(customer);
});
// POST ny kund (Uppgift 7)
app.MapPost("/api/customers", async (CreateCustomerDto dto,
FailSafeRentalsDbContext _db) =>
{
    var customer = new Customer
    {
        FullName = dto.FullName,
        Email = dto.Email,
        DriverLicenseNumber = dto.DriverLicenseNumber
    };
    _db.Customers.Add(customer);
    await _db.SaveChangesAsync();
    var resultDto = new CustomerDto(
        customer.Id,
        customer.FullName,
        customer.Email,
        customer.DriverLicenseNumber
    );
    return Results.Created($"/api/customers/{customer.Id}", resultDto);
});
#endregion

#region /api/rentals
// POST ny uthyrning med domänvalidering & prisberäkning (Uppgift 14)
app.MapPost("/api/rentals", async (CreateRentalDto dto,
FailSafeRentalsDbContext db) =>
{
    if (dto.EndDate <= dto.StartDate)
        return Results.Problem("Slutdatum måste ligga efter startdatum.", statusCode: 400);
        var customer = await db.Customers.FindAsync(dto.CustomerId);
    if (customer is null)
        return Results.Problem($"Kund med ID {dto.CustomerId} hittades inte.", statusCode: 404);
        var vehicle = await db.Vehicles.FindAsync(dto.VehicleId);
    if (vehicle is null)
        return Results.Problem($"Fordon med ID {dto.VehicleId} hittades inte.", statusCode: 404);
if (!vehicle.IsAvailable)
        return Results.Problem($"Fordonet {vehicle.RegistrationNumber} är redan uthyrt.", statusCode: 400);
        var days = (dto.EndDate.Date - dto.StartDate.Date).Days;
    if (days < 1) days = 1;
    var totalCost = days * vehicle.DailyRate;
    var rental = new Rental
    {
        CustomerId = dto.CustomerId,
        VehicleId = dto.VehicleId,
        StartDate = dto.StartDate,
        EndDate = dto.EndDate,
        TotalCost = totalCost
    };
    
    vehicle.IsAvailable = false;
    
    db.Rentals.Add(rental);
    await db.SaveChangesAsync();
    
    var resultDto = new RentalDto(
        rental.Id,
        rental.CustomerId,
        rental.VehicleId,
        rental.StartDate,
        (DateTime)rental.EndDate,
        rental.TotalCost
    );
    return Results.Created($"/api/rentals/{rental.Id}", resultDto);
});
#endregion

app.Run();

// DTO- och andra records MÅSTE ligga sist (egentligen efter app.run)
#region DTOs
internal record VehicleDto(
    int Id,
    string RegistrationNumber,
    string Brand,
    string Model,
    decimal DailyRate,
    bool IsAvailable
    );
internal record CreateVehicleDto(
    string RegistrationNumber,
    string Brand,
    string Model,
    decimal DailyRate
    );

internal record CustomerDto(
    int Id,
    string FullName,
    string Email,
    string DriverLicenseNumber
    );
internal record CreateCustomerDto(
    string FullName,
    string LastName,
    string Email,
    string DriverLicenseNumber
    );
internal record RentalDto(
    int Id,
    int CustomerId,
    int VehicleId,
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalCost
    );
internal record CreateRentalDto(
    int CustomerId,
    int VehicleId,
    DateTime StartDate,
    DateTime EndDate
    );
internal record ExtraFeatureDto(
    int Id,
    string Name,
    decimal DailyPrice
    );
internal record CreateExtraFeatureDto(
    string Name,
    decimal DailyPrice
    );
#endregion