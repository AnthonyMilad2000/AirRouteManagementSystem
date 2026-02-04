using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AirRouteManagementSystem.DTOs.Request
{
    public class ValidateOtpRequest
    {
        [Required]
        public string OTP { get; set; } = string.Empty;
    }
}
