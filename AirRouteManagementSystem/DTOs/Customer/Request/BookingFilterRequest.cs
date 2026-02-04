using System;

namespace AirRouteManagementSystem.DTOs.Customer.Request
{
    public class BookingFilterRequest
    {
        public string? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
