using AirRouteManagementSystem.DTOs.Request;
using AirRouteManagementSystem.DTOs.Response;
using AirRouteManagementSystem.Model;
using AirRouteManagementSystem.Repository.IRepository;
using AirRouteManagementSystem.Services;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AirRouteManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{SD.SuperAdminRole}, {SD.AdminRole},{SD.EmployeeRole}")]
    public class FlightController : ControllerBase
    {
        private IRepository<Flight> _flightRepository;
        private IRepository<Airport> _airportRepository;
        private IUnitOfWork _unitOfWork;
        private IRepository<FlightPrice> _flightPriceRepository;
        private IFlightPriceRepository _iFlightPriceRepository;
        private ILocationService _locationService;

        public FlightController(IRepository<Flight> flightRepository, IUnitOfWork unitOfWork, IRepository<FlightPrice> flightPriceRepository, IFlightPriceRepository iFlightPriceRepository, IRepository<Airport> airportRepository, ILocationService locationService)
        {
            _flightRepository=flightRepository;
            _unitOfWork=unitOfWork;
            _flightPriceRepository= flightPriceRepository;
            _iFlightPriceRepository=iFlightPriceRepository;
            _airportRepository=airportRepository;
            _locationService=locationService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int page = 1,int pageSize = 10,string? search = null,CancellationToken cancellationToken = default)
        {
            var flights = await _flightRepository.GetAsync(Include: [ e => e.FlightPrice ],cancellationToken: cancellationToken);

            var query = flights.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(f => f.FlightNumber.Contains(search));
            }

            var total = query.Count();

            var data = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PagedResponse<Flight>
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id, CancellationToken cancellationToken)
        {
            var flight = await _flightRepository.GetAsync(e => e.Id == id, Include: [e => e.FlightPrice], cancellationToken: cancellationToken);
            if (flight is null)
                return NotFound(new ErrorModel
                {
                    Code = "Flight Not Found",
                    Description = "Flight Not Found"
                });
            return Ok(flight);
        }




        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FlightRequest flightRequest, CancellationToken cancellationToken)
        {
            var fromAirport = await _airportRepository.GetOneAsync(a => a.Id == flightRequest.FromAirportId);
            var toAirport = await _airportRepository.GetOneAsync(a => a.Id == flightRequest.ToAirportId);

            if (fromAirport == null || toAirport == null)
            {
                return BadRequest(new { Code = "InvalidAirport", Description = "From or To Airport not found" });
            }

            var distance = _locationService.CalculateDistance(
                fromAirport.Latitude,
                fromAirport.Longitude,
                toAirport.Latitude,
                toAirport.Longitude
            );

            var flight = flightRequest.Adapt<Flight>();
            flight.FlightNumber = Guid.NewGuid().ToString().Substring(0, 10);
            flight.Distance = distance; 

            var flightCreated = await _flightRepository.CreateAsync(flight, cancellationToken);
            await _unitOfWork.Commit();

            if (flightRequest.FlightPrice is not null)
            {
                foreach (var priceRequest in flightRequest.FlightPrice)
                {
                    var flightPrice = priceRequest.Adapt<FlightPrice>();
                    flightPrice.FlightId = flightCreated.Id;

                    await _flightPriceRepository.CreateAsync(flightPrice, cancellationToken);
                }

                await _unitOfWork.Commit();
            }

            return CreatedAtAction(nameof(GetOne), new { id = flightCreated.Id },
                new SuccessModel
                {
                    Code = "Flight Created",
                    Description = "Airport Flight Successfully"
                });
        }

        [Authorize(Roles = $"{SD.SuperAdminRole}, {SD.AdminRole}")]

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FlightRequest flightRequest, CancellationToken cancellationToken)
        {
            var flight = await _flightRepository.GetOneAsync(
                e => e.Id == id,
                Include: new Expression<Func<Flight, object>>[] { e => e.FlightPrice },
                cancellationToken: cancellationToken
            );

            if (flight is null)
                return NotFound(new ErrorModel
                {
                    Code = "Flight Not Found",
                    Description = "Flight Not Found"
                });

            flight.FromAirportId = flightRequest.FromAirportId;
            flight.ToAirportId   = flightRequest.ToAirportId;

            var fromAirport = await _airportRepository.GetOneAsync(a => a.Id == flightRequest.FromAirportId, cancellationToken: cancellationToken);
            var toAirport = await _airportRepository.GetOneAsync(a => a.Id == flightRequest.ToAirportId, cancellationToken: cancellationToken);

            if (fromAirport != null && toAirport != null)
            {
                flight.Distance = _locationService.CalculateDistance(
                    fromAirport.Latitude,
                    fromAirport.Longitude,
                    toAirport.Latitude,
                    toAirport.Longitude
                );
            }

            flight.DepartureTime = flightRequest.DepartureTime;
            flight.ArrivalTime   = flightRequest.ArrivalTime;
            flight.Duration      = flightRequest.Duration;
            flight.Status        = flightRequest.Status;

            var requestPrices = flightRequest.FlightPrice ?? new List<FlightPriceRequest>();
            var requestIds = requestPrices.Where(p => p.Id > 0).Select(p => p.Id).ToList();
            var pricesToDelete = flight.FlightPrice.Where(p => !requestIds.Contains(p.Id)).ToList();

            if (pricesToDelete.Any())
            {
                _iFlightPriceRepository.RemoveRange(pricesToDelete);
            }

            foreach (var priceRequest in requestPrices)
            {
                if (priceRequest.Id > 0)
                {
                    var existingPrice = flight.FlightPrice.FirstOrDefault(p => p.Id == priceRequest.Id);
                    if (existingPrice == null) continue;

                    existingPrice.SeatClass = priceRequest.SeatClass;
                    existingPrice.MinSeats  = priceRequest.MinSeats;
                    existingPrice.MaxSeats  = priceRequest.MaxSeats;
                    existingPrice.Price     = priceRequest.Price;
                }
                else
                {
                    var newPrice = new FlightPrice
                    {
                        FlightId  = flight.Id,
                        SeatClass = priceRequest.SeatClass,
                        MinSeats  = priceRequest.MinSeats,
                        MaxSeats  = priceRequest.MaxSeats,
                        Price     = priceRequest.Price
                    };

                    await _flightPriceRepository.CreateAsync(newPrice, cancellationToken);
                }
            }

            await _unitOfWork.Commit();

            return Ok(new SuccessModel
            {
                Code = "Flight Updated",
                Description = "Flight updated successfully"
            });
        }
        [Authorize(Roles = $"{SD.SuperAdminRole}, {SD.AdminRole}")]

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var flight = await _flightRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);
            if (flight is null)
                return NotFound(new ErrorModel
                {
                    Code = "Flight Not Found",
                    Description = "Flight Not Found",
                });
            _flightRepository.Remove(flight);
            await _unitOfWork.Commit();

            return Ok(new SuccessModel
            {
                Code = "Flight Delete",
                Description = "Flight Deleted Successfully"
            });
        }
    }
}
