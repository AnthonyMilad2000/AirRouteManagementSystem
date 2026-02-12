using AirRouteManagementSystem.DTOs.Customer.Request;
using AirRouteManagementSystem.DTOs.Customer.Response;
using AirRouteManagementSystem.Model;
using AirRouteManagementSystem.Model.Customer;
using AirRouteManagementSystem.Repository.IRepository;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace AirRouteManagementSystem.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Route("[area]/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly IRepository<Flight> _flightRepository;

        public FlightController(IRepository<Flight> flightRepository)
        {
            _flightRepository = flightRepository;
        }

        // GET: /Customer/Flight
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var flights = await _flightRepository.GetAsync(
                 Include: [f => f.FromAirport, f => f.ToAirport, f=>f.FlightPrice],
                tracking: false,
                cancellationToken: cancellationToken
            );

            //var response = flights
            //    .Select(f => f.Adapt<FlightCustomerResponse>())
            //    .AsQueryable();

            var response = flights.AsQueryable();


            return Ok(response);
        }

        // POST: /Customer/Flight/search
        [HttpPost("search")]
        public async Task<IActionResult> Search(
            FlightCustomerRequest request,
            CancellationToken cancellationToken
        )
        {
            var flights = await _flightRepository.GetAsync(
                f =>
                    f.FromAirportId == request.FromAirportId &&
                    f.ToAirportId == request.ToAirportId &&
                    f.DepartureTime.Date == request.Date.Date,
                tracking: false,
                cancellationToken: cancellationToken
            );

            var response = flights
                .Select(f => f.Adapt<FlightResponse>())
                .AsQueryable();

            return Ok(response);
        }

        // POST: /Customer/Flight/Filter

        [HttpPost("filter")]
        public async Task<IActionResult> Filter(
            FlightFilterRequest request,
            CancellationToken cancellationToken
)
        {
            var flights = await _flightRepository.GetAsync(
                f =>
                    (!request.FromAirportId.HasValue || f.FromAirportId == request.FromAirportId) &&
                    (!request.ToAirportId.HasValue || f.ToAirportId == request.ToAirportId) &&
                    (!request.Date.HasValue || f.DepartureTime.Date == request.Date.Value.Date) &&
                    (!request.MinPrice.HasValue || (f.FlightPrice != null && f.FlightPrice.Min(fp => fp.Price) >= request.MinPrice)) &&
                    (!request.MaxPrice.HasValue || (f.FlightPrice != null && f.FlightPrice.Max(fp => fp.Price) <= request.MaxPrice)),
                tracking: false,
                cancellationToken: cancellationToken
            );

            // Sorting
            flights = request.SortBy?.ToLower() switch
            {
                "price" => request.IsDescending
                    ? flights.OrderByDescending(f => f.FlightPrice != null && f.FlightPrice.Any() ? f.FlightPrice.Min(fp => fp.Price) : 0)
                    : flights.OrderBy(f => f.FlightPrice != null && f.FlightPrice.Any() ? f.FlightPrice.Min(fp => fp.Price) : 0),

                "date" => request.IsDescending
                    ? flights.OrderByDescending(f => f.DepartureTime)
                    : flights.OrderBy(f => f.DepartureTime),

                _ => flights.OrderBy(f => f.Id)
            };


            var totalCount = flights.Count();

            // Pagination
            var pagedFlights = flights
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(f => f.Adapt<FlightResponse>())
                .ToList();

            return Ok(new
            {
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                Data = pagedFlights
            });
        }


    }
}
