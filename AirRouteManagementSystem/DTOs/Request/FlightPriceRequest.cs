using AirRouteManagementSystem.Model;

namespace AirRouteManagementSystem.DTOs.Request
{
    public class FlightPriceRequest
    {
        public int Id { get; set; }  
        public SeatClass SeatClass { get; set; }
        public int MinSeats { get; set; }
        public int MaxSeats { get; set; }
        public decimal Price { get; set; }

    }
}
