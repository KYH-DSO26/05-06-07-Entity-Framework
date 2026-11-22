using Microsoft.EntityFrameworkCore;

using FailSafeRentals.Core.Models;
using Microsoft.Extensions.Logging;

namespace FailSafeRentals.Data;

public class FailSafeRentalsDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Vehicle> Vehicles { get; set; } = null!;
    public DbSet<Rental> Rentals { get; set; } = null!;
    public DbSet<ExtraFeature> ExtraFeatures { get; set; } = null!;
    

    public FailSafeRentalsDbContext()
    {
        
    }
    public FailSafeRentalsDbContext(DbContextOptions<FailSafeRentalsDbContext> options) 
        : base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\MSSQLLocalDB;Database=FailSafeRentals;Trusted_Connection=True;TrustServerCertificate=True;")
            //Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=FailSafeIT;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30
                //.LogTo(Console.WriteLine, LogLevel.Information)
                //.EnableSensitiveDataLogging()   // Endast i dev - visar parametervärden!
                ;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. Unikt index på registreringsnummer
        modelBuilder.Entity<Vehicle>()
            .HasIndex(v => v.RegistrationNumber)
            .IsUnique();

        // 2. Explicit precision för pengar i LocalDB
        //    Sök på "public decimal" för att hitta alla
        modelBuilder.Entity<Vehicle>()
            .Property(v => v.DailyRate)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Rental>()
            .Property(v => v.TotalCost)
            .HasPrecision(10, 2);

        modelBuilder.Entity<ExtraFeature>()
            .Property(v => v.DailyPrice)
            .HasPrecision(10, 2);

        // 3. Shadow Property för SecOps-spårbarhet
        modelBuilder.Entity<Rental>()
            .Property<DateTime>("CreatedAt")
            .HasDefaultValueSql("GETUTCDATE()");

        // 4. Skydda uthyrningshistorik mot oavsiktlig radering
        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Customer)
            .WithMany(c => c.Rentals)
            .OnDelete(DeleteBehavior.Restrict);

        // 5. Lägg till lite data:
        // 1. Seed Customers
        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, FullName = "Anna Lindqvist", Email = "anna.lindqvist@example.se", DriverLicenseNumber = "19850412-1234" },
            new Customer { Id = 2, FullName = "Erik Svensson", Email = "erik.svensson@example.se", DriverLicenseNumber = "19901123-5678" },
            new Customer { Id = 3, FullName = "Maria Berg", Email = "maria.berg@example.se", DriverLicenseNumber = "19780205-9012" },
            new Customer { Id = 4, FullName = "Johan Ekström", Email = "johan.ekstrom@example.se", DriverLicenseNumber = "19950719-3456" },
            new Customer { Id = 5, FullName = "Sofia Holm", Email = "sofia.holm@example.se", DriverLicenseNumber = "19880930-7890" }
        );

        // 2. Seed Vehicles
        modelBuilder.Entity<Vehicle>().HasData(
            new Vehicle { Id = 1, RegistrationNumber = "ABC234", Brand = "Volvo", Model = "C30", DailyRate = 850m, IsAvailable = true },
            new Vehicle { Id = 2, RegistrationNumber = "DEF567", Brand = "Polestar", Model = "2", DailyRate = 1200m, IsAvailable = false },
            new Vehicle { Id = 3, RegistrationNumber = "GHI890", Brand = "Volkswagen", Model = "Golf", DailyRate = 700m, IsAvailable = true },
            new Vehicle { Id = 4, RegistrationNumber = "JKL123", Brand = "BMW", Model = "320i", DailyRate = 1100m, IsAvailable = true },
            new Vehicle { Id = 5, RegistrationNumber = "MNO456", Brand = "Toyota", Model = "Yaris", DailyRate = 550m, IsAvailable = false }
        );

        // 3. Seed ExtraFeatures
        modelBuilder.Entity<ExtraFeature>().HasData(
            new ExtraFeature { Id = 1, Name = "GPS-navigator", DailyPrice = 75m },
            new ExtraFeature { Id = 2, Name = "Barnstol", DailyPrice = 100m },
            new ExtraFeature { Id = 3, Name = "Takbox", DailyPrice = 150m },
            new ExtraFeature { Id = 4, Name = "Vinterdäck", DailyPrice = 50m },
            new ExtraFeature { Id = 5, Name = "Extra försäkring", DailyPrice = 200m }
        );

        // 4. Seed Rentals (Kopplar ihop Customer och Vehicle via FK)
        modelBuilder.Entity<Rental>().HasData(
            new Rental { Id = 1, CustomerId = 1, VehicleId = 1, StartDate = new DateTime(2026, 1, 10), EndDate = new DateTime(2026, 1, 13), TotalCost = 2550m },
            new Rental { Id = 2, CustomerId = 2, VehicleId = 2, StartDate = new DateTime(2026, 11, 1), EndDate = null, TotalCost = 0m }, // Pågående uthyrning (matchar IsAvailable = false)
            new Rental { Id = 3, CustomerId = 3, VehicleId = 3, StartDate = new DateTime(2026, 2, 15), EndDate = new DateTime(2026, 2, 17), TotalCost = 1400m },
            new Rental { Id = 4, CustomerId = 4, VehicleId = 4, StartDate = new DateTime(2026, 3, 5), EndDate = new DateTime(2026, 3, 10), TotalCost = 5500m },
            new Rental { Id = 5, CustomerId = 5, VehicleId = 5, StartDate = new DateTime(2026, 11, 18), EndDate = null, TotalCost = 0m }  // Pågående uthyrning (matchar IsAvailable = false)
        );

        // Bonus: Seed Many-to-Many mellan Vehicle och ExtraFeature (Skip navigation)
        modelBuilder.Entity<Vehicle>()
            .HasMany(v => v.ExtraFeatures)
            .WithMany(e => e.Vehicles)
            .UsingEntity(j => j.HasData(
                new { VehiclesId = 1, ExtraFeaturesId = 1 },
                new { VehiclesId = 1, ExtraFeaturesId = 4 },
                new { VehiclesId = 2, ExtraFeaturesId = 5 },
                new { VehiclesId = 3, ExtraFeaturesId = 2 },
                new { VehiclesId = 4, ExtraFeaturesId = 3 }
            ));

    }
}
