using System;

namespace AirRouteManagementSystem.DTOs.Customer.Request
{
    public class FlightFilterRequest
    {
        public int? FromAirportId { get; set; }
        public int? ToAirportId { get; set; }

        public DateTime? Date { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public string? SortBy { get; set; } // price | date
        public bool IsDescending { get; set; } = false;

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
