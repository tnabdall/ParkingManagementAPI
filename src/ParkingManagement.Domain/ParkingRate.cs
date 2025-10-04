using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingManagement.Domain
{
    public class ParkingRate
    {
        public int ParkingLotId { get; set; }
        public VehicleType VehicleType { get; set; }
        public double RatePerMinute { get; set; }
        public double AdditionalRatePerFiveMinutes { get; set; }
    }
}