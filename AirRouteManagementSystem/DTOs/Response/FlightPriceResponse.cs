using AirRouteManagementSystem.Model;

namespace AirRouteManagementSystem.DTOs.Response
{
    public class FlightPriceResponse
    {
        public int Id { get; set; }
        public SeatClass SeatClass { get; set; }
        public int MinSeats { get; set; }
        public int MaxSeats { get; set; }
        public decimal Price { get; set; }
    }
}
