namespace AirRouteManagementSystem.DTOs.Request
{
    public class AirportRequest
    {
        public string Name { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

    }
}
