namespace AirRouteManagementSystem.DTOS.Response
{
    public class ErrorModel
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ReturnedAt { get; set; } = DateTime.UtcNow;
    }
}
