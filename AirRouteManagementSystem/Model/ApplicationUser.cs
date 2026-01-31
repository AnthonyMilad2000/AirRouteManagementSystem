using Microsoft.AspNetCore.Identity;

namespace AirRouteManagementSystem.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }=string.Empty;
        public string Address { get; set; }=string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
