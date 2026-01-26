namespace AirRouteManagementSystem.Model
{
    public enum SeatClass
    {
        Economy,
        Business,
        FirstClass
    }

    public class FlightPrice
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public Flight Flight { get; set; } = null!;
        public SeatClass SeatClass { get; set; }
        public int MinSeats { get; set; }
        public int MaxSeats { get; set; }
        public decimal Price { get; set; }
    }
}
