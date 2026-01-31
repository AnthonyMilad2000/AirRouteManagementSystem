using AirRouteManagementSystem.Model;

namespace AirRouteManagementSystem.DTOs.Request
{
    public class AircraftRequest
    {
        public string AircraftCode { get; set; } = string.Empty;

        public AircraftType AircraftType { get; set; }
        public CapacityType CapacityType { get; set; }

        public int Capacity { get; set; }
        public decimal MaxRangeKm { get; set; }
        public decimal MaxWeight { get; set; }
        public IFormFile? Image { get; set; }

    }
}
