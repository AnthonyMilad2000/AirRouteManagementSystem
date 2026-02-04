using System;

namespace AirRouteManagementSystem.DTOs.Customer.Request
{
    public class FlightCustomerRequest
    {
        public int FromAirportId { get; set; }
        public int ToAirportId { get; set; }
        public DateTime Date { get; set; }
    }
}
