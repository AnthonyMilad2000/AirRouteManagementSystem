namespace AirRouteManagementSystem.Model
{
    public enum SeatClass
    {
        Economy = 1,
        Business = 2,
        FirstClass = 3
    }

    public class FlightPrice
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public SeatClass SeatClass { get; set; }
        public int MinSeats { get; set; }
        public int MaxSeats { get; set; }
        public decimal Price { get; set; }
    }
}
