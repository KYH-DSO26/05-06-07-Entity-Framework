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
                .LogTo(Console.WriteLine, new[]
                {
                    DbLoggerCategory.Database.Command.Name
                }
                , LogLevel.Information)
                .EnableSensitiveDataLogging()   // Endast i dev - visar parametervärden!
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
        modelBuilder.Entity<Vehicle>()
            .Property(v => v.DailyRate)
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

        

    }
}
