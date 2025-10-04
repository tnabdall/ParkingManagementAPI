using Microsoft.EntityFrameworkCore;
using ParkingManagement.Domain;

namespace ParkingManagement.Services;

public class ParkingManagementService(IParkingManagementDbContext context) : IParkingManagementService
{
    public async Task<ParkingAllocation> AllocateParkingSpaceAsync(int parkingLotId, string vehicleRegistrationNumber, VehicleType vehicleType)
    {
        var existingAllocation = await context.VehicleParkingAllocations
            .FirstOrDefaultAsync(vpa => vpa.ParkingLotId == parkingLotId && vpa.VehicleRegistrationNumber == vehicleRegistrationNumber && vpa.ParkingEndTime == null);

        if (existingAllocation != null)
        {
            throw new InvalidOperationException("Vehicle is already parked.");
        }

        var availableSpace = await context.ParkingSpaces
            .FirstOrDefaultAsync(ps => ps.ParkingLotId == parkingLotId && !ps.IsOccupied)
            ?? throw new InvalidOperationException("No available parking space found.");

        availableSpace.IsOccupied = true;

        var allocation = new ParkingAllocation
        {
            VehicleRegistrationNumber = vehicleRegistrationNumber,            
            VehicleType = vehicleType,
            ParkingLotId = parkingLotId,
            ParkingSpaceNumber = availableSpace.ParkingSpaceNumber,
            ParkingStartTime = DateTime.UtcNow,
            ParkingEndTime = null
        };

        context.VehicleParkingAllocations.Add(allocation);
        await context.SaveChangesAsync();

        return allocation;
    }

    private async Task<ParkingRate> GetParkingRateAsync(int parkingLotId, VehicleType vehicleType)
    {
        var parkingRate = await context.VehicleParkingRates
            .FirstOrDefaultAsync(rate => rate.ParkingLotId == parkingLotId && rate.VehicleType == vehicleType)
            ?? throw new InvalidOperationException("Parking rate not found.");
        return parkingRate;
    }    

    public async Task<double> CalculateParkingChargeAsync(int parkingLotId, VehicleType vehicleType, DateTime startTime, DateTime endTime)
    {
        var rate = await GetParkingRateAsync(parkingLotId, vehicleType);
        var totalCharge = RateCalculator.CalculateParkingFee(startTime, endTime, rate.RatePerMinute, rate.AdditionalRatePerFiveMinutes);
        return totalCharge;
    }

    public Task<List<ParkingSpace>> GetParkingSpacesAsync(int parkingLotId)
    {
        return context.ParkingSpaces
            .Where(ps => ps.ParkingLotId == parkingLotId)
            .ToListAsync();
    }

    public async Task<ParkingAllocation> ReleaseParkingSpaceAsync(int parkingLotId, string vehicleRegistrationNumber)
    {
        var allocation = await context.VehicleParkingAllocations
            .FirstOrDefaultAsync(vpa => vpa.ParkingLotId == parkingLotId && vpa.VehicleRegistrationNumber == vehicleRegistrationNumber && vpa.ParkingEndTime == null)
            ?? throw new InvalidOperationException("Active parking allocation not found.");

        var parkingSpace = await context.ParkingSpaces
            .FirstOrDefaultAsync(ps => ps.ParkingLotId == parkingLotId && ps.ParkingSpaceNumber == allocation.ParkingSpaceNumber)
            ?? throw new InvalidOperationException("Parking space not found.");

        allocation.ParkingEndTime = DateTime.UtcNow;
        parkingSpace.IsOccupied = false;
        await context.SaveChangesAsync();

        return allocation;
    }
}
