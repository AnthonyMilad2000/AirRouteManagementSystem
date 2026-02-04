using System.ComponentModel.DataAnnotations;

namespace AirRouteManagementSystem.DTOs.Request
{
    public class LoginDTORequest
    {
        [Required, Display(Name = "User Name Or Password")]
        public string UsernameOrEmail { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
