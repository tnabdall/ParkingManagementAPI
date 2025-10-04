using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingManagement.API.DTOs
{
    public record ParkingAllocationCreated(string VehicleReg, int SpaceNumber, DateTime TimeIn);
}