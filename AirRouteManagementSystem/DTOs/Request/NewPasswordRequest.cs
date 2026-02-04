using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AirRouteManagementSystem.DTOs.Request
{
    public class NewPasswordRequest
    {
        [Required(ErrorMessage ="Enter New Password")]
        [DataType(DataType.Password)]
        public string Newpassword { get; set; }=string.Empty;
        [Required(ErrorMessage = "Enter Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Newpassword")]
        public string Confirmpassword { get; set; }=string.Empty ;

    }
}
