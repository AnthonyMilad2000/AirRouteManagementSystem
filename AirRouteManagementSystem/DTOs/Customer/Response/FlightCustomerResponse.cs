using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirRouteManagementSystem.DTOs.Customer.Response
{
    public class FlightCustomerResponse
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;

        public int FromAirportId { get; set; }

        public int ToAirportId { get; set; }

        public string FromImgUrl { get; set; }
        public string toomImgUrl { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        public string Duration { get; set; } = string.Empty; 

        public double Distance { get; set; }

        public List<FlightPrice>? FlightPrices { get; set; }
    }
}
