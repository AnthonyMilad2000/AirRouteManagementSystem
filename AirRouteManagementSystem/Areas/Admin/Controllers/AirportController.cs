using AirRouteManagementSystem.DTOs.Request;
using AirRouteManagementSystem.DTOs.Response;
using AirRouteManagementSystem.Model;
using AirRouteManagementSystem.Repository.IRepository;
using AirRouteManagementSystem.Services;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace AirRouteManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{SD.SuperAdminRole}, {SD.AdminRole},{SD.EmployeeRole}")]

    public class AirportController : ControllerBase
    {
        private IRepository<Airport> _airportRepository;
        private IUnitOfWork _UnitWork;
        private readonly ILocationService _locationService;


        public AirportController(IRepository<Airport> airportRepository, IUnitOfWork unitOfWork,ILocationService locationService)
        {
            _airportRepository=airportRepository;
            _UnitWork=unitOfWork;
            _locationService =locationService;
        }

        [HttpGet]
        public async Task<IActionResult> Get( int page = 1,int pageSize = 10,string? search = null, CancellationToken cancellationToken = default)
        {
          
            var airports = await _airportRepository.GetAsync(tracking: false, cancellationToken: cancellationToken);
            var query = airports.AsQueryable();
            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a => a.Name.Contains(search));
            }

            var total = query.Count();

            var data = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PagedResponse<Airport>
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id,CancellationToken cancellationToken)
        {
            var airport = await _airportRepository.GetOneAsync(e=>e.Id == id ,cancellationToken: cancellationToken);
            if(airport is null)
                return NotFound(new ErrorModel
                {
                    Code = "Airport Not Found",
                    Description = "Airport Not Found"
                });

            return Ok(airport);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] AirportRequest airportRequest, CancellationToken cancellationToken)
        {
            var airport = airportRequest.Adapt<Airport>();

            if (!string.IsNullOrEmpty(airportRequest.LocationLink))
            {
                var coords = _locationService.ExtractCoordinates(airportRequest.LocationLink);

                if (coords != null)
                {
                    airport.Latitude = coords.Value.lat;
                    airport.Longitude = coords.Value.lng;
                }
            }

            if (airportRequest.Image is not null && airportRequest.Image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(airportRequest.Image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\airport_images", fileName);

               
                using (var stream = System.IO.File.Create(filePath))
                {
                    airportRequest.Image.CopyTo(stream);
                }

                airport.Image = fileName;
            }
            await _airportRepository.CreateAsync(airport, cancellationToken);
            await _UnitWork.Commit();
            return CreatedAtAction(nameof(GetOne), new { airport.Id}, new SuccessModel
            {
                Code = "Airport Created",
                Description = "Airport Created Successfully"
            });
        }

        [Authorize(Roles = $"{SD.SuperAdminRole}, {SD.AdminRole}")]

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] AirportRequest airportRequest, CancellationToken cancellationToken)
        {
            var airportInDb = await _airportRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);
            if (airportInDb is null)
                return NotFound(new ErrorModel
                {
                    Code = "Airport Not Found",
                    Description = "Airport Not Found !!"
                });

            if (airportRequest.Image != null && airportRequest.Image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(airportRequest.Image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\airport_images", fileName);

                var oldImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\airport_images", airportInDb.Image);
                if (System.IO.File.Exists(oldImgPath))
                {
                    System.IO.File.Delete(oldImgPath);
                }

                using (var stream = System.IO.File.Create(filePath))
                {
                    airportRequest.Image.CopyTo(stream);
                }

                airportInDb.Image = fileName;
            }

            if (!string.IsNullOrEmpty(airportRequest.LocationLink))
            {
                var coords = _locationService.ExtractCoordinates(airportRequest.LocationLink);
                if (coords != null)
                {
                    airportInDb.Latitude  = coords.Value.lat;
                    airportInDb.Longitude = coords.Value.lng;
                }
            }
            else
            {
                airportInDb.Latitude  = airportRequest.Latitude;
                airportInDb.Longitude = airportRequest.Longitude;
            }

            airportInDb.Name = airportRequest.Name;

            _airportRepository.Update(airportInDb);
            await _UnitWork.Commit();

            return NoContent();
        }

        [Authorize(Roles = $"{SD.SuperAdminRole}, {SD.AdminRole}")]

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var airport = await _airportRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);
            if (airport is null)
                return NotFound(new ErrorModel
                {
                    Code = "Airport Not Found",
                    Description = "Airport Not Found",
                });

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "airport_images");
            var oldImgPath = Path.Combine(uploadsFolder, airport.Image ?? "");

            if (airport.Image is not null && System.IO.File.Exists(oldImgPath))
            {
                System.IO.File.Delete(oldImgPath);
            }

            _airportRepository.Remove(airport);
            await _UnitWork.Commit();

            return NoContent();
        }
    }
}
