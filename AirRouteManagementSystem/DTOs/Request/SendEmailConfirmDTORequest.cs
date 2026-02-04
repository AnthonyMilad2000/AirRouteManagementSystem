using System.ComponentModel.DataAnnotations;

namespace AirRouteManagementSystem.DTOs.Request
{
    public class SendEmailConfirmDTORequest
    {
        [Required(ErrorMessage ="Please Enter Your Or UserName")]
        public string usernameOrEmail { get; set; }=string.Empty;  
    }
}
