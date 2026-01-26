namespace AirRouteManagementSystem.Model
{
    public class Airport
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Image { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
