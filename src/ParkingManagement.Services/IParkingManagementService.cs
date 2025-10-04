using ParkingManagement.Domain;

namespace ParkingManagement.Services;

public interface IParkingManagementService
{
    Task<List<ParkingSpace>> GetParkingSpacesAsync(int parkingLotId);
    Task<ParkingAllocation> AllocateParkingSpaceAsync(int parkingLotId, string vehicleRegistrationNumber, VehicleType vehicleType);
    Task<ParkingAllocation> ReleaseParkingSpaceAsync(int parkingLotId, string vehicleRegistrationNumber);
    Task<double> CalculateParkingChargeAsync(int parkingLotId, VehicleType vehicleType, DateTime startTime, DateTime endTime);
}