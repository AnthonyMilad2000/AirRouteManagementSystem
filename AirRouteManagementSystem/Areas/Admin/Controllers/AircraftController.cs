
using AirRouteManagementSystem.Repository.IRepository;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace AirRouteManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/[controller]")]
    [ApiController]
    public class AircraftController : ControllerBase
    {
        private IRepository<Aircraft> _aircraftRepository;
        private IUnitOfWork _unitOfWork;
        private IAircraftSubImagesRepository _aircraftSubImagesRepository;

        public AircraftController(IRepository<Aircraft> aircraftRepository, IUnitOfWork unitOfWork, IAircraftSubImagesRepository aircraftSubImagesRepository)
        {
            _aircraftRepository=aircraftRepository;
            _unitOfWork=unitOfWork;
            _aircraftSubImagesRepository=aircraftSubImagesRepository;
        }


        [HttpGet]
        public async Task<IActionResult> Get( int page = 1, int pageSize = 10,string? search = null,
      CancellationToken cancellationToken = default)
        {
            var aircrafts = await _aircraftRepository.GetAsync(tracking: false, cancellationToken: cancellationToken);
            var query = aircrafts.AsQueryable();
            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a => a.AircraftCode.Contains(search)
                                      || a.AircraftCode.Contains(search));
            }

          

            var total = query.Count();

            var data = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PagedResponse<Aircraft>
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
            var aircraft = await _aircraftRepository.GetOneAsync(e=>e.Id == id, Include: [e=>e.SubImages!],cancellationToken: cancellationToken);
            if (aircraft is null)
                return NotFound(new ErrorModel
                {
                    Code = "Aircraft Not Found",
                    Description = "Aircraft Not Found"
                });
            return Ok(aircraft);
        }

        [HttpPost]
        public async Task<IActionResult> Create( AircraftRequest aircraftRequest, List<IFormFile> SubImgs, CancellationToken cancellationToken) 
        {
            var aircraft = aircraftRequest.Adapt<Aircraft>();
            if (aircraftRequest.Image is not null && aircraftRequest.Image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(aircraftRequest.Image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\aircraft_images", fileName);


                using (var stream = System.IO.File.Create(filePath))
                {
                    aircraftRequest.Image.CopyTo(stream);
                }

                aircraft.Image = fileName;
            }

            aircraft.AircraftCode = Guid.NewGuid().ToString().Substring(0, 10);
            var aircraftCreated = await _aircraftRepository.CreateAsync(aircraft, cancellationToken: cancellationToken);
            await _unitOfWork.Commit();

            if (SubImgs is not null && SubImgs.Count > 0)
            {
                foreach (var item in SubImgs)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\aircraft_images", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        item.CopyTo(stream);
                    }

                    await _aircraftSubImagesRepository.CreateAsync(new()
                    {
                        SubImagePath = fileName,
                        AirCraftId = aircraftCreated.Id
                    }, cancellationToken: cancellationToken);
                }
                await _unitOfWork.Commit();
            }

            return CreatedAtAction(nameof(GetOne), new { aircraft.Id }, new SuccessModel
            {
                Code = "Aircraft Create",
                Description = "Aircraft Created Successfully",
            });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, AircraftRequest aircraftRequest, List<IFormFile> SubImgs, CancellationToken cancellationToken)
        {
            var aircraftInDb = await _aircraftRepository.GetOneAsync(
                e => e.Id == id,
                Include: [e => e.SubImages!],
                cancellationToken: cancellationToken
            );

            if (aircraftInDb is null)
                return NotFound(new ErrorModel
                {
                    Code = "Aircraft Not Found",
                    Description = "Aircraft Not Found !!"
                });

            if (aircraftRequest.Image != null && aircraftRequest.Image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(aircraftRequest.Image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "aircraft_images", fileName);

                if (!string.IsNullOrEmpty(aircraftInDb.Image))
                {
                    var oldImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "aircraft_images", aircraftInDb.Image);
                    if (System.IO.File.Exists(oldImgPath))
                    {
                        System.IO.File.Delete(oldImgPath);
                    }
                }

                using (var stream = System.IO.File.Create(filePath))
                {
                    await aircraftRequest.Image.CopyToAsync(stream, cancellationToken);
                }

                aircraftInDb.Image = fileName;
            }

            if (SubImgs is not null && SubImgs.Count > 0)
            {
                foreach (var item in SubImgs)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(item.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "aircraft_images", fileName);

                    using var stream = System.IO.File.Create(filePath);
                    await item.CopyToAsync(stream, cancellationToken);

                    await _aircraftSubImagesRepository.CreateAsync(new AirCraftSubImg
                    {
                        SubImagePath = fileName,
                        AirCraftId = aircraftInDb.Id
                    }, cancellationToken);
                }
            }

            aircraftInDb.AircraftType = aircraftRequest.AircraftType;
            aircraftInDb.CapacityType = aircraftRequest.CapacityType;
            aircraftInDb.Capacity = aircraftRequest.Capacity;
            aircraftInDb.MaxRangeKm = aircraftRequest.MaxRangeKm;
            aircraftInDb.MaxWeight = aircraftRequest.MaxWeight;

            _aircraftRepository.Update(aircraftInDb);
            await _unitOfWork.Commit();

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var aircraft = await _aircraftRepository.GetOneAsync(
                e => e.Id == id,
                Include: [e => e.SubImages],
                cancellationToken: cancellationToken
            );

            if (aircraft is null)
                return NotFound();

            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "aircraft_images"
            );

            if (aircraft.SubImages is not null && aircraft.SubImages.Any())
            {
                foreach (var subImg in aircraft.SubImages)
                {
                    var subImgPath = Path.Combine(uploadsFolder, subImg.SubImagePath);
                    if (System.IO.File.Exists(subImgPath))
                    {
                        System.IO.File.Delete(subImgPath);
                    }
                }
            }

            if (!string.IsNullOrEmpty(aircraft.Image))
            {
                var mainImgPath = Path.Combine(uploadsFolder, aircraft.Image);
                if (System.IO.File.Exists(mainImgPath))
                {
                    System.IO.File.Delete(mainImgPath);
                }
            }

            _aircraftRepository.Remove(aircraft);
            await _unitOfWork.Commit();

            return NoContent();
        }

    }
}
