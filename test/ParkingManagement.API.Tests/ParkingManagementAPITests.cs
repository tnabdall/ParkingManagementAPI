using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using ParkingManagement.API; // Add this line if your Program class is in the ParkingManagement.API namespace

namespace ParkingManagement.API.Tests;

public class ParkingManagementAPITests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ParkingManagementAPITests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetParkingSpacesAvailability_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/parking");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        // Optionally check response content
        var summary = await response.Content.ReadFromJsonAsync<object>();
        Assert.NotNull(summary);
    }

    [Fact]
    public async Task AllocateParkingSpace_ReturnsOk_WhenValid()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsync($"/parking?vehicleReg=TEST123&vehicleType=Small", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var allocation = await response.Content.ReadFromJsonAsync<object>();
        Assert.NotNull(allocation);
    }

    [Fact]
    public async Task AllocateParkingSpace_ReturnsBadRequest_WhenInvalidVehicleType()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsync($"/parking?vehicleReg=TEST123&vehicleType=InvalidType", null);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReleaseParkingSpace_ReturnsOk_WhenValid()
    {
        var client = _factory.CreateClient();
        // First allocate a space
        await client.PostAsync($"/parking?vehicleReg=RELEASE123&vehicleType=Small", null);
        // Then release it
        var response = await client.PostAsync($"/parking/exit?vehicleReg=RELEASE123", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var released = await response.Content.ReadFromJsonAsync<object>();
        Assert.NotNull(released);
    }

    [Fact]
    public async Task ReleaseParkingSpace_ReturnsBadRequest_WhenNotAllocated()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsync($"/parking/exit?vehicleReg=NOTFOUND123", null);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}