using AirRouteManagementSystem.Model;
using AirRouteManagementSystem.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AirRouteManagementSystem.DTOs.Response;
using AirRouteManagementSystem.DTOs.Request;
using Mapster;
namespace AirRouteManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/[controller]")]
    [ApiController]

    public class AirportController : ControllerBase
    {
        private IRepository<Airport> _airportRepository;
        private IUnitOfWork _UnitWork;

        public AirportController(IRepository<Airport> airportRepository, IUnitOfWork unitOfWork)
        {
            _airportRepository=airportRepository;
            _UnitWork=unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var airports = await _airportRepository.GetAsync(tracking: false,cancellationToken: cancellationToken);
            return Ok(airports.AsQueryable());
        }
        [HttpGet("id")]
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


        [HttpPut("id")]
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
            airportInDb.Longitude = airportRequest.Longitude;
            airportInDb.Latitude = airportRequest.Latitude;
            airportInDb.Name = airportRequest.Name;
            _airportRepository.Update(airportInDb);
            await _UnitWork.Commit();
            return NoContent();
        }

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
