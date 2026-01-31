

namespace AirRouteManagementSystem.DTOs.Request
{
    public class ChatRequestDto
    {
        [Required(ErrorMessage = "Enter your question")]
        public string Question { get; set; }=string.Empty;
    }
}
