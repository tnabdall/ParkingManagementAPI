using ParkingManagement.API.DTOs;
using ParkingManagement.Domain;
using ParkingManagement.Services;

namespace ParkingManagement.API;

public static class ParkingEndpoints
{
    // Assuming a single parking lot with ID 1 for simplicity
    public const int ParkingLotId = 1;

    public static void MapParkingEndpoints(this WebApplication app)
    {
        app.MapGet("/parking", async (IParkingManagementService service) =>
        {
            try
            {
                var spaces = await service.GetParkingSpacesAsync(ParkingLotId);
                var availableSpaces = spaces.Count(ps => !ps.IsOccupied);
                var occupiedSpaces = spaces.Count - availableSpaces;
                return Results.Ok(new ParkingSpaceAvailabilitySummary(availableSpaces, occupiedSpaces));
            }
            catch (Exception ex)
            {
                // log the exception here
                Console.Error.WriteLine(ex);
                return Results.Problem("Unable to retrieve parking spaces");
            }
        })
        .WithName("GetParkingSpacesAvailability")
        .WithOpenApi();

        app.MapPost("/parking", async (IParkingManagementService service, string vehicleReg, string vehicleType) =>
        {
            try
            {
                if (!Enum.TryParse<VehicleType>(vehicleType, true, out var vehicleTypeEnum))
                {
                    return Results.BadRequest("Invalid vehicle type");
                }
                var allocation = await service.AllocateParkingSpaceAsync(ParkingLotId, vehicleReg, vehicleTypeEnum);
                return Results.Ok(new ParkingAllocationCreated(allocation.VehicleRegistrationNumber, allocation.ParkingSpaceNumber, allocation.ParkingStartTime));
            }
            catch (Exception ex)
            {
                // log the exception here
                Console.Error.WriteLine(ex);
                return Results.BadRequest("Unable to allocate parking space");
            }
        })
        .WithName("AllocateParkingSpace")
        .WithOpenApi();

        app.MapPost("/parking/exit", async (IParkingManagementService service, string vehicleReg) =>
        {
            try
            {
                var allocation = await service.ReleaseParkingSpaceAsync(ParkingLotId, vehicleReg);

                var endTime = allocation.ParkingEndTime ?? DateTime.UtcNow;
                if (!allocation.ParkingEndTime.HasValue)
                {
                    // Log error: Parking end time should be set after releasing the space
                    // We'll just use current time for calculation and not charge the user extra
                    Console.Error.WriteLine("Parking end time not set for allocation: " + allocation.VehicleRegistrationNumber + " at space " + allocation.ParkingSpaceNumber);                    
                }

                var totalCharge = await service.CalculateParkingChargeAsync(ParkingLotId, allocation.VehicleType, allocation.ParkingStartTime, endTime);
                return Results.Ok(new ParkingAllocationReleased(allocation.VehicleRegistrationNumber, totalCharge, allocation.ParkingStartTime, endTime));
            }
            catch (Exception ex)
            {
                // log the exception here
                Console.Error.WriteLine(ex);
                return Results.BadRequest("Unable to release parking space");
            }
        })
        .WithName("ReleaseParkingSpace")
        .WithOpenApi();
    }
}