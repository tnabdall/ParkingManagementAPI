using Microsoft.EntityFrameworkCore;
using ParkingManagement.Domain;
using ParkingManagement.Infrastructure;


namespace ParkingManagement.Services.Tests;

public class ParkingManagementServiceTests
{
	private ParkingManagementDbContext CreateDbContext()
	{
		var options = new DbContextOptionsBuilder<ParkingManagementDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new ParkingManagementDbContext(options);
	}

	[Fact]
	public async Task AllocateParkingSpaceAsync_Allocates_WhenAvailable()
	{
		using var context = CreateDbContext();
		context.ParkingSpaces.Add(new ParkingSpace { ParkingLotId = 1, ParkingSpaceNumber = 1, IsOccupied = false });
		await context.SaveChangesAsync();

		var service = new ParkingManagementService(context);
		var result = await service.AllocateParkingSpaceAsync(1, "ABC123", VehicleType.Small);

		Assert.NotNull(result);
		Assert.Equal("ABC123", result.VehicleRegistrationNumber);
		Assert.True(context.ParkingSpaces.First().IsOccupied);
	}

	[Fact]
	public async Task AllocateParkingSpaceAsync_Throws_WhenAlreadyParked()
	{
		using var context = CreateDbContext();
		context.ParkingSpaces.Add(new ParkingSpace { ParkingLotId = 1, ParkingSpaceNumber = 1, IsOccupied = false });
		context.VehicleParkingAllocations.Add(new ParkingAllocation { ParkingLotId = 1, VehicleRegistrationNumber = "ABC123", ParkingEndTime = null });
		await context.SaveChangesAsync();

		var service = new ParkingManagementService(context);
		await Assert.ThrowsAnyAsync<Exception>(() => service.AllocateParkingSpaceAsync(1, "ABC123", VehicleType.Small));
	}

	[Fact]
	public async Task AllocateParkingSpaceAsync_Throws_WhenNoSpaceAvailable()
	{
		using var context = CreateDbContext();
		await context.SaveChangesAsync();

		var service = new ParkingManagementService(context);
		await Assert.ThrowsAnyAsync<Exception>(() => service.AllocateParkingSpaceAsync(1, "XYZ789", VehicleType.Small));
	}

	[Fact]
	public async Task ReleaseParkingSpaceAsync_Releases_WhenActiveAllocationExists()
	{
		using var context = CreateDbContext();
		context.ParkingSpaces.Add(new ParkingSpace { ParkingLotId = 1, ParkingSpaceNumber = 1, IsOccupied = true });
		context.VehicleParkingAllocations.Add(new ParkingAllocation { ParkingLotId = 1, VehicleRegistrationNumber = "ABC123", ParkingSpaceNumber = 1, ParkingEndTime = null });
		await context.SaveChangesAsync();

		var service = new ParkingManagementService(context);
		var result = await service.ReleaseParkingSpaceAsync(1, "ABC123");

		Assert.NotNull(result.ParkingEndTime);
		Assert.False(context.ParkingSpaces.First().IsOccupied);
	}

	[Fact]
	public async Task ReleaseParkingSpaceAsync_Throws_WhenAllocationNotFound()
	{
		using var context = CreateDbContext();
		context.ParkingSpaces.Add(new ParkingSpace { ParkingLotId = 1, ParkingSpaceNumber = 1, IsOccupied = true });
		await context.SaveChangesAsync();

		var service = new ParkingManagementService(context);
		await Assert.ThrowsAnyAsync<Exception>(() => service.ReleaseParkingSpaceAsync(1, "ABC123"));
	}

	[Fact]
	public async Task ReleaseParkingSpaceAsync_Throws_WhenSpaceNotFound()
	{
		using var context = CreateDbContext();
		context.VehicleParkingAllocations.Add(new ParkingAllocation { ParkingLotId = 1, VehicleRegistrationNumber = "ABC123", ParkingSpaceNumber = 99, ParkingEndTime = null });
		await context.SaveChangesAsync();

		var service = new ParkingManagementService(context);
		await Assert.ThrowsAnyAsync<Exception>(() => service.ReleaseParkingSpaceAsync(1, "ABC123"));
	}

	[Fact]
	public async Task GetParkingSpacesAsync_ReturnsSpaces()
	{
		using var context = CreateDbContext();
		context.ParkingSpaces.Add(new ParkingSpace { ParkingLotId = 1, ParkingSpaceNumber = 1 });
		context.ParkingSpaces.Add(new ParkingSpace { ParkingLotId = 1, ParkingSpaceNumber = 2 });
		await context.SaveChangesAsync();

		var service = new ParkingManagementService(context);
		var result = await service.GetParkingSpacesAsync(1);
		Assert.Equal(2, result.Count);
	}

	[Theory]
	[InlineData("Small", 0, 0)]
	[InlineData("Small", 1, 0.1)]
	[InlineData("Small", 5, 1.5)]
	[InlineData("Small", 9.40732, 1.94)]
	[InlineData("Small", 59, 16.9)]
	[InlineData("Small", 60, 18.0)]
	[InlineData("Medium", 0, 0)]
	[InlineData("Medium", 1, 0.2)]
	[InlineData("Medium", 5, 2.0)]
	[InlineData("Medium", 9.40732, 2.88)]
	[InlineData("Medium", 59, 22.8)]
	[InlineData("Medium", 60, 24.0)]
	[InlineData("Large", 0, 0)]
	[InlineData("Large", 1, 0.4)]
	[InlineData("Large", 5, 3.0)]
	[InlineData("Large", 9.40732, 4.76)]
	[InlineData("Large", 59, 34.6)]
	[InlineData("Large", 60, 36.0)]
	public async Task CalculateParkingCharge_ReturnsAppropriateCharge(string vehicleType, double minutesParked, double expectedCharge)
	{
		using var context = CreateDbContext();
		var rates = new List<ParkingRate>
		{
			new ParkingRate
			{
				ParkingLotId = 1,
				VehicleType = VehicleType.Small,
				RatePerMinute = 0.1,
				AdditionalRatePerFiveMinutes = 1
			},
			new ParkingRate
			{
				ParkingLotId = 1,
				VehicleType = VehicleType.Medium,
				RatePerMinute = 0.2,
				AdditionalRatePerFiveMinutes = 1
			},
			new ParkingRate
			{
				ParkingLotId = 1,
				VehicleType = VehicleType.Large,
				RatePerMinute = 0.4,
				AdditionalRatePerFiveMinutes = 1
			}
		};
		context.VehicleParkingRates.AddRange(rates);
		await context.SaveChangesAsync();

		var vType = Enum.Parse<VehicleType>(vehicleType);
		var startTime = DateTime.UtcNow;
		var endTime = startTime.AddMinutes(minutesParked);

		var service = new ParkingManagementService(context);
		var charge = await service.CalculateParkingChargeAsync(1, vType, startTime, endTime);
		Assert.Equal(expectedCharge, charge);
	}

	[Fact]
	public async Task CalculateParkingCharge_Throws_WhenAllocationNotFound()
	{
		using var context = CreateDbContext();
		var service = new ParkingManagementService(context);
		await Assert.ThrowsAnyAsync<Exception>(() => service.CalculateParkingChargeAsync(1, VehicleType.Small, DateTime.UtcNow, DateTime.UtcNow.AddHours(1)));
	}

	[Fact]
	public async Task CalculateParkingCharge_Throws_WhenParkingEndTimeIsBeforeStartTime()
	{
		using var context = CreateDbContext();
		var rates = new List<ParkingRate>
		{
			new ParkingRate
			{
				ParkingLotId = 1,
				VehicleType = VehicleType.Small,
				RatePerMinute = 0.1,
				AdditionalRatePerFiveMinutes = 1
			},
			new ParkingRate
			{
				ParkingLotId = 1,
				VehicleType = VehicleType.Medium,
				RatePerMinute = 0.2,
				AdditionalRatePerFiveMinutes = 1
			},
			new ParkingRate
			{
				ParkingLotId = 1,
				VehicleType = VehicleType.Large,
				RatePerMinute = 0.4,
				AdditionalRatePerFiveMinutes = 1
			}			
		};
		context.VehicleParkingRates.AddRange(rates);
		await context.SaveChangesAsync();

		var service = new ParkingManagementService(context);
		await Assert.ThrowsAnyAsync<Exception>(() => service.CalculateParkingChargeAsync(1, VehicleType.Small, DateTime.UtcNow, DateTime.UtcNow.AddHours(-1)));
	}
}