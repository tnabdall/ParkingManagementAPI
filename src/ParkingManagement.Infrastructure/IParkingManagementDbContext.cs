using Microsoft.EntityFrameworkCore;
using ParkingManagement.Domain;

public interface IParkingManagementDbContext
{
    DbSet<ParkingSpace> ParkingSpaces { get; set; }
    DbSet<ParkingAllocation> VehicleParkingAllocations { get; set; }
    DbSet<ParkingRate> VehicleParkingRates { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}