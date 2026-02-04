namespace AirRouteManagementSystem.DTOs.Customer.Response
{
    public class CartResponse
    {
        public int CartId { get; set; }
        public int FlightId { get; set; }
        public string FlightNumber { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
