using AirRouteManagementSystem.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirRouteManagementSystem.DTOs.Response
{
    public class FlightResponse
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;

        public int FromAirportId { get; set; }
      
        public int ToAirportId { get; set; }

       
        public double Distance { get; set; }

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        public TimeSpan Duration { get; set; }

        public FlightStatus Status { get; set; }
        public List<FlightPriceResponse> FlightPrice { get; set; } = [];

        public Airport Airport { get; set; }
    }
}
