using ParkingManagement.Domain;
using ParkingManagement.Infrastructure;

namespace ParkingManagement.API;

public static class DatabaseSeeder
{
    public static void Seed(ParkingManagementDbContext context)
    {
        if (!context.ParkingSpaces.Any())
        {
            // Add unoccupied spaces with numbers ranging from 1 to 100
            context.ParkingSpaces.AddRange(
                Enumerable.Range(1, 100)
                    .Select(i => new ParkingSpace
                    {
                        ParkingLotId = 1,
                        ParkingSpaceNumber = i,
                        IsOccupied = false
                    })
            );
        }

        if (!context.VehicleParkingRates.Any())
        {
            context.VehicleParkingRates.AddRange(new[] {
                new ParkingRate { ParkingLotId = 1, VehicleType = VehicleType.Small, RatePerMinute = 0.10, AdditionalRatePerFiveMinutes = 1 },
                new ParkingRate { ParkingLotId = 1, VehicleType = VehicleType.Medium, RatePerMinute = 0.20, AdditionalRatePerFiveMinutes = 1 },
                new ParkingRate { ParkingLotId = 1, VehicleType = VehicleType.Large, RatePerMinute = 0.40, AdditionalRatePerFiveMinutes = 1 }
            });
        }

        context.SaveChanges();
    }
}
