using AirRouteManagementSystem.DTOs.Request;
using AirRouteManagementSystem.DTOs.Response;
using AirRouteManagementSystem.Model;
using AirRouteManagementSystem.Repository.IRepository;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AirRouteManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private IRepository<Flight> _flightRepository;
        private IUnitOfWork _unitOfWork;
        private IRepository<FlightPrice> _flightPriceRepository;

        public FlightController(IRepository<Flight> flightRepository, IUnitOfWork unitOfWork, IRepository<FlightPrice> flightPriceRepository)
        {
            _flightRepository=flightRepository;
            _unitOfWork=unitOfWork;
            _flightPriceRepository= flightPriceRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
     int page = 1,
     int pageSize = 10,
     string? search = null,
     CancellationToken cancellationToken = default)
        {
            var query = (await _flightRepository.GetAsync(Include: new Expression<Func<Flight, object>>[] { e => e.FlightPrice },
                                                           cancellationToken: cancellationToken))
                        .AsQueryable();

            // Search على رقم الرحلة أو أي حقل تحبه
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
            var flight = await _flightRepository.GetAsync(e=>e.Id == id,Include:[e=>e.FlightPrice], cancellationToken: cancellationToken);
            if(flight is null)
                return NotFound(new ErrorModel
                {
                    Code = "Flight Not Found",
                    Description = "Flight Not Found"
                });
            return Ok(flight);
        }




        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FlightRequest flightRequest,CancellationToken cancellationToken)
        {
            var flight = flightRequest.Adapt<Flight>();
            flight.FlightNumber = Guid.NewGuid().ToString().Substring(0, 10);

            var flightCreated = await _flightRepository.CreateAsync(flight, cancellationToken);
            await _unitOfWork.Commit();

            if (flightRequest.FlightPrice is not null )
            {
                foreach (var priceRequest in flightRequest.FlightPrice)
                {
                    var flightPrice = priceRequest.Adapt<FlightPrice>();
                    flightPrice.FlightId = flightCreated.Id;

                    await _flightPriceRepository.CreateAsync(flightPrice,cancellationToken);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id,[FromBody] FlightRequest flightRequest, CancellationToken cancellationToken)
        {
            var flight = await _flightRepository.GetOneAsync(
                e => e.Id == id,
                Include: [ e => e.FlightPrice ],
                cancellationToken: cancellationToken
            );

            if (flight is null)
                return NotFound(new ErrorModel
                {
                    Code = "Flight Not Found",
                    Description = "Flight Not Found"
                });

            flight.FromAirportId = flightRequest.FromAirportId;
            flight.ToAirportId = flightRequest.ToAirportId;
            flight.Distance = flightRequest.Distance;
            flight.DepartureTime = flightRequest.DepartureTime;
            flight.ArrivalTime = flightRequest.ArrivalTime;
            flight.Duration = flightRequest.Duration;
            flight.Status = flightRequest.Status;

            if (flightRequest.FlightPrice is not null && flightRequest.FlightPrice.Any())
            {
                foreach (var priceRequest in flightRequest.FlightPrice)
                {
                    if (priceRequest.Id > 0)
                    {
                        var existingPrice = flight.FlightPrice
                            .FirstOrDefault(p => p.Id == priceRequest.Id);

                        if (existingPrice is null)
                            continue;

                        existingPrice.SeatClass = priceRequest.SeatClass;
                        existingPrice.MinSeats  = priceRequest.MinSeats;
                        existingPrice.MaxSeats  = priceRequest.MaxSeats;
                        existingPrice.Price     = priceRequest.Price;
                    }
                    else
                    {
                        var newPrice = priceRequest.Adapt<FlightPrice>();
                        newPrice.FlightId = flight.Id;

                        await _flightPriceRepository.CreateAsync(
                            newPrice,
                            cancellationToken
                        );
                    }
                }
            }

            await _unitOfWork.Commit();

            return Ok(new SuccessModel
            {
                Code = "Flight Updated",
                Description = "Flight updated successfully"
            });
        }

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
