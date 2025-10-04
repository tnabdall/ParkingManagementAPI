using Microsoft.EntityFrameworkCore;
using ParkingManagement.API;
using ParkingManagement.Infrastructure;
using ParkingManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register EF Core InMemory DbContext
builder.Services.AddDbContext<ParkingManagementDbContext>(options =>
    options.UseInMemoryDatabase("ParkingManagementDb"));

builder.Services.AddScoped<IParkingManagementDbContext>(provider => provider.GetRequiredService<ParkingManagementDbContext>());
builder.Services.AddScoped<IParkingManagementService, ParkingManagementService>();

var app = builder.Build();

// Seed the database after building the app
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ParkingManagementDbContext>();
    DatabaseSeeder.Seed(dbContext);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapParkingEndpoints();


app.Run();

// For integration testing with WebApplicationFactory<Program>
namespace ParkingManagement.API
{
    public partial class Program { }
}