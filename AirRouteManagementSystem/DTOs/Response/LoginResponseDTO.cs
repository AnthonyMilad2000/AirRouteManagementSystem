namespace AirRouteManagementSystem.DTOs.Response
{
    public class LoginResponseDTO
    {
        public bool IsSuccess { get; set; }
        public string Status { get; set; }  = string.Empty;
        public string? Message { get; set; }

        public string? AccessToken { get; set; }
        public string ExpiresAt { get; set; }=string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
