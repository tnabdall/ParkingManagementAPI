using Microsoft.EntityFrameworkCore;
using ParkingManagement.Domain;

namespace ParkingManagement.Infrastructure
{
    public class ParkingManagementDbContext : DbContext, IParkingManagementDbContext
    {
        public ParkingManagementDbContext(DbContextOptions<ParkingManagementDbContext> options)
            : base(options) { }

        public DbSet<ParkingSpace> ParkingSpaces { get; set; }
        public DbSet<ParkingAllocation> VehicleParkingAllocations { get; set; }
        public DbSet<ParkingRate> VehicleParkingRates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ParkingSpace>()
                .HasKey(ps => new { ps.ParkingLotId, ps.ParkingSpaceNumber });

            modelBuilder.Entity<ParkingAllocation>()
                .Property(pa => pa.ParkingAllocationId)
                .ValueGeneratedOnAdd()
                .IsRequired();

            modelBuilder.Entity<ParkingAllocation>()
                .HasKey(pa => pa.ParkingAllocationId);

            modelBuilder.Entity<ParkingRate>()
                .HasKey(pr => new { pr.ParkingLotId, pr.VehicleType });
        }
    }
}