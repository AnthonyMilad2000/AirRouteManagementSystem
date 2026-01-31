using System.ComponentModel.DataAnnotations.Schema;

namespace AirRouteManagementSystem.Model
{
    public enum FlightStatus
    {
        Scheduled,
        Delayed,
        Cancelled,
        Completed
    }

    public class Flight
    {
        public int Id { get; set; } 

        public string FlightNumber { get; set; } = string.Empty;
        [ForeignKey("FromAirport")]

        public int FromAirportId { get; set; }   
        public Airport FromAirport { get; set; } = null!;
        [ForeignKey("ToAirport")]

        public int ToAirportId { get; set; }     

        public Airport ToAirport { get; set; } = null!;

        public double Distance { get; set; }     

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        public TimeSpan Duration { get; set; }

        public FlightStatus Status { get; set; }

        public List<FlightPrice>? FlightPrice { get; set; }
    }
}
