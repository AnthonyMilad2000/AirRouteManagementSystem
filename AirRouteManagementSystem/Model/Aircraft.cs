namespace AirRouteManagementSystem.Model
{
    public enum AircraftType
    {
        VIP,
        Classic
    }

    public enum CapacityType
    {
        Small,
        Medium,
        Large
    }

    public class Aircraft
    {
        public int Id { get; set; }
        public string AircraftCode { get; set; } = string.Empty;

        public AircraftType Type { get; set; }
        public CapacityType CapacityType { get; set; }

        public int Capacity { get; set; }
        public decimal MaxRangeKm { get; set; }
        public decimal MaxWeight { get; set; }

        public List<AirCraftSubImg>? SubImages { get; set; }
    }
}
