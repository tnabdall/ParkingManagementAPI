using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingManagement.Domain
{
    public class ParkingSpace
    {
        public int ParkingLotId { get; set; }
        public int ParkingSpaceNumber { get; set; }
        public bool IsOccupied { get; set; }

        // This could have an AllowedVehicleTypes property to indicate what type of vehicles can park here
    }
}