namespace ParkingManagement.API.DTOs
{
    public record ParkingAllocationReleased(string VehicleReg, double VehicleCharge, DateTime TimeIn, DateTime TimeOut);
}