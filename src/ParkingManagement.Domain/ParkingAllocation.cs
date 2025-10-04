using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingManagement.Domain
{
    public class ParkingAllocation
    {
        public int ParkingAllocationId { get; set; }
        public string VehicleRegistrationNumber { get; set; }
        public VehicleType VehicleType { get; set; }
        public int ParkingLotId { get; set; }
        public int ParkingSpaceNumber { get; set; }
        public DateTime ParkingStartTime { get; set; }
        public DateTime? ParkingEndTime { get; set; }
    }
}