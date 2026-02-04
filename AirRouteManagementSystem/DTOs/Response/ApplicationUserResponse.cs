namespace AirRouteManagementSystem.DTOs.Response
{
    public class ApplicationUserResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
        [Compare("NewPassword"), DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
        public string? Role { get; set; }
    }
}
