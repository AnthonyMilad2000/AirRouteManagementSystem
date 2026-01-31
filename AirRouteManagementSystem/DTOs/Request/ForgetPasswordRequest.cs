using System.ComponentModel.DataAnnotations;

namespace AirRouteManagementSystem.DTOs.Request
{
    public class ForgetPasswordRequest
    {
        [Required(ErrorMessage ="Enter Email Or userName")]
        public string UsernameOrEmail { get; set; }
    }
}
