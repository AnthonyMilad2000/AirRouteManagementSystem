namespace AirRouteManagementSystem.DTOs.Customer.Response
{
    public class BookingResponse
    {
        public int BookingId { get; set; }
        public decimal SubPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalPrice { get; set; }
        public string Status { get; set; }
    }
}
