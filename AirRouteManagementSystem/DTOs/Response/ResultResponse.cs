namespace AirRouteManagementSystem.DTOs.Response
{
    public class ResultResponse
    {
        public bool Stutas {  get; set; }
        public string Message { get; set; }=string.Empty;
        public string? UserId { get; set; }
        public string? Any { get; set; }
    }
}
