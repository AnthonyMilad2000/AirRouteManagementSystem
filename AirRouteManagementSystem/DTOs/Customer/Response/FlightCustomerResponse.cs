using System;

namespace AirRouteManagementSystem.DTOs.Customer.Response
{
	public class FlightCustomerResponse
	{
		public int Id { get; set; }
		public string FlightNumber { get; set; }

		public DateTime DepartureTime { get; set; }
		public DateTime ArrivalTime { get; set; }

		public decimal Price { get; set; }
	}
}
