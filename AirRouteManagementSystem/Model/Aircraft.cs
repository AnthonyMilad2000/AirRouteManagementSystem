namespace AirRouteManagementSystem.Model
{
    public enum AircraftType
    {
        Economy = 1,
        Classic = 2,
        VIP = 3,
    }

    public enum CapacityType
    {
        Small = 1,
        Medium = 2,
        Large = 3,
    }

    public class Aircraft
    {
        public int Id { get; set; }
        public string AircraftCode { get; set; } = string.Empty;

        public AircraftType AircraftType { get; set; }
        public CapacityType CapacityType { get; set; }

        public int Capacity { get; set; }
        public decimal MaxRangeKm { get; set; }
        public decimal MaxWeight { get; set; }
        public string? Image {  get; set; }

        public List<AirCraftSubImg>? SubImages { get; set; }
    }
}
