using Microsoft.EntityFrameworkCore;
using RealEstateApp.Model;

public class RealEstateDbContext : DbContext
{
    public RealEstateDbContext(DbContextOptions<RealEstateDbContext> options)
        : base(options)
    { }

    public DbSet<Property> Properties { get; set; }
    public DbSet<Owner> Owners { get; set; }
    public DbSet<RentalContract> RentalContracts { get; set; } // Make sure this is included
    public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; } // Make sure this is included
    // Add other DbSet properties as necessary for your other entities
}


