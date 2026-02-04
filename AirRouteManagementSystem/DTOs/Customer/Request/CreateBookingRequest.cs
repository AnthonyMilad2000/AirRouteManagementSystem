namespace AirRouteManagementSystem.DTOs.Customer.Request
{
    public class CreateBookingRequest
    {
        public int CartId { get; set; }
        public string? PromotionCode { get; set; }
    }
}
